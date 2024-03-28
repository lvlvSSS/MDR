namespace MDR.Core.Task.Pipeline;

public class PipelineBuilder : IPipelineBuilder
{
    private readonly List<Func<PipelineDelegate, PipelineDelegate>> _components = new();

    public PipelineDelegate Build()
    {
        PipelineDelegate pipeline = task => System.Threading.Tasks.Task.CompletedTask;
        for (var c = _components.Count - 1; c >= 0; c--)
        {
            pipeline = _components[c](pipeline);
        }

        return pipeline;
    }

    public IPipelineBuilder Use(Func<PipelineDelegate, PipelineDelegate> @delegate)
    {
        _components.Add(@delegate);
        return this;
    }
}