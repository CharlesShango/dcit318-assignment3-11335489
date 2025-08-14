using System;
using System.Collections.Generic;
using System.Linq;

// Generic Repository for Entity Management
public class Repository<T>
{
    private List<T> _items = new List<T>();

    public void Add(T item)
    {
        _items.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_items); // Return a copy to maintain encapsulation
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return _items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = GetById(predicate);
        if (item != null)
        {
            return _items.Remove(item);
        }
        return false;
    }
}

// Patient class
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }
}

// Prescription class
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Patient ID: {PatientId}, Medication: {MedicationName}, Date: {DateIssued:yyyy-MM-dd}";
    }
}

// Health System Application
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(new Patient(1, "John Doe", 45, "Male"));
        _patientRepo.Add(new Patient(2, "Jane Smith", 32, "Female"));
        _patientRepo.Add(new Patient(3, "Robert Johnson", 60, "Male"));

        // Add prescriptions
        var today = DateTime.Today;
        _prescriptionRepo.Add(new Prescription(101, 1, "Ibuprofen", today.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(102, 1, "Amoxicillin", today.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(103, 2, "Lisinopril", today.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(104, 2, "Metformin", today));
        _prescriptionRepo.Add(new Prescription(105, 3, "Atorvastatin", today.AddDays(-1)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();

        var allPrescriptions = _prescriptionRepo.GetAll();
        foreach (var prescription in allPrescriptions)
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
        {
            return prescriptions;
        }
        return new List<Prescription>();
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("All Patients:");
        Console.WriteLine("------------");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
        Console.WriteLine();
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient == null)
        {
            Console.WriteLine($"Patient with ID {patientId} not found.");
            return;
        }

        Console.WriteLine($"Prescriptions for {patient.Name}:");
        Console.WriteLine("---------------------------------");

        var prescriptions = GetPrescriptionsByPatientId(patientId);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
        else
        {
            foreach (var prescription in prescriptions)
            {
                Console.WriteLine(prescription);
            }
        }
        Console.WriteLine();
    }
}

// Main Program
class Program
{
    static void Main(string[] args)
    {
        var healthSystem = new HealthSystemApp();

        // Seed initial data
        healthSystem.SeedData();

        // Build prescription map
        healthSystem.BuildPrescriptionMap();

        // Print all patients
        healthSystem.PrintAllPatients();

        // Print prescriptions for a specific patient
        Console.WriteLine("Enter Patient ID to view prescriptions (1-3):");
        if (int.TryParse(Console.ReadLine(), out int patientId))
        {
            healthSystem.PrintPrescriptionsForPatient(patientId);
        }
        else
        {
            Console.WriteLine("Invalid input. Showing prescriptions for Patient ID 1 as example.");
            healthSystem.PrintPrescriptionsForPatient(1);
        }
    }
}