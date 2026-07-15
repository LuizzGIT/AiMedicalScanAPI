public class DiagnosticDetails
{
    public required int HeartRate {get; set;}

    public bool? AnomalyDetected {get; set;}

    public required List<string> MedicalObservations {get; set;}
}