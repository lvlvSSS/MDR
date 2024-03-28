using MDR.Data.Model.Domains;

namespace MDR.Core.Task.Pipeline;

public interface IPrescriptionTaskHandler
{
    System.Threading.Tasks.Task Invoke(PrescriptionTask prescriptionTask, PipelineDelegate next);
}