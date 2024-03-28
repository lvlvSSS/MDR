using MDR.Data.Model.Domains;

namespace MDR.Core.Task.Pipeline;

public delegate System.Threading.Tasks.Task PipelineDelegate(PrescriptionTask prescriptionTask);