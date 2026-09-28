using System.Threading.Channels;
using Minicraft.Engine.Geometry;

namespace Minicraft.Engine.World.Chunks;

public sealed class ChunkScheduler : IDisposable
{
    private readonly Func<ChunkPosition, ChunkSnapshot> _generate;
    private readonly Func<ChunkMeshInput, MeshData> _mesh;

    private readonly Channel<IChunkWorkRequest> _requests = Channel.CreateUnbounded<IChunkWorkRequest>();
    private readonly Channel<IChunkWorkResult> _results = Channel.CreateUnbounded<IChunkWorkResult>();
    
    private readonly CancellationTokenSource _cts = new();
    private readonly Task[] _workers;

    public ChunkScheduler(
        Func<ChunkPosition, ChunkSnapshot> generate,
        Func<ChunkMeshInput, MeshData> mesh,
        int workerCount)
    {
        if (workerCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(workerCount));
        
        _generate = generate;
        _mesh = mesh;
        
        _workers = new Task[workerCount];

        for (var i = 0; i < workerCount; i++)
            _workers[i] = WorkerLoopAsync();
    }

    public void RequestGenerate(ChunkPosition position)
    {
        _requests.Writer.TryWrite(new GenerateRequest(position));
    }

    public void RequestMesh(ChunkPosition position, ChunkMeshInput input)
    {
        _requests.Writer.TryWrite(new MeshRequest(position, input));
    }

    public bool TryGetCompleted(out IChunkWorkResult? result)
    {
        return _results.Reader.TryRead(out result);
    }
    
    // ---

    private async Task WorkerLoopAsync()
    {
        try
        {
            await foreach (var req in _requests.Reader.ReadAllAsync(_cts.Token))
            {
                IChunkWorkResult res = req switch
                {
                    GenerateRequest generateReq =>
                        new GenerateResult(generateReq.Position, _generate(generateReq.Position)),

                    MeshRequest meshReq =>
                        new MeshResult(meshReq.Position, _mesh(meshReq.Input)),

                    _ => throw new InvalidOperationException($"Unknown work request: {req.GetType().Name}")
                };

                await _results.Writer.WriteAsync(res, _cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Shutdown
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _requests.Writer.TryComplete();

        try
        {
            Task.WaitAll(_workers);
        }
        catch (AggregateException)
        {
            // Workers canceled during shutdown.
        }
        
        _results.Writer.TryComplete();
        _cts.Dispose();
    }
}