namespace MDR.Core.Task.Pipeline;

public interface IPipelineBuilder
{
    PipelineDelegate Build();
    IPipelineBuilder Use(Func<PipelineDelegate, PipelineDelegate> @delegate);
}