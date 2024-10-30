using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

abstract class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }

    public virtual void Register() => Console.WriteLine($"{Name} has been registered.");

    public virtual void DisplayProfile() =>
        Console.WriteLine($"User ID: {UserId}, Name: {Name}, Phone Number: {PhoneNumber}");
}

class Rider : User
{
    public List<Trip> RideHistory { get; set; } = new List<Trip>();

    public override void DisplayProfile()
    {
        base.DisplayProfile();
        Console.WriteLine("User Type: Rider");
        Console.WriteLine("Ride History:");
        ViewRideHistory();
    }

    public void ViewRideHistory()
    {
        if (RideHistory.Count == 0)
            Console.WriteLine("No trips found.");
        else
            foreach (var trip in RideHistory)
                trip.DisplayTripDetails();
    }
}

class Driver : User
{
    public int DriverID { get; set; }
    public string VehicleDetails { get; set; }
    public bool IsAvailable { get; set; } = true;
    public List<Trip> TripHistory { get; set; } = new List<Trip>();

    public override void Register()
    {
        base.Register();
        Console.WriteLine($"Driver ID: {DriverID}, Vehicle: {VehicleDetails}");
    }

    public override void DisplayProfile()
    {
        base.DisplayProfile();
        Console.WriteLine("User Type: Driver");
        Console.WriteLine($"Driver ID: {DriverID}, Vehicle Details: {VehicleDetails}, Availability: {IsAvailable}");
        Console.WriteLine("Trip History:");
        ViewTripHistory();
    }

    public void ToggleAvailability()
    {
        IsAvailable = !IsAvailable;
        Console.WriteLine($"{Name} is now {(IsAvailable ? "available" : "unavailable")}.");
    }

    public void ViewTripHistory()
    {
        if (TripHistory.Count == 0)
            Console.WriteLine("No trips found.");
        else
            foreach (var trip in TripHistory)
                trip.DisplayTripDetails();
    }
}

class Trip
{
    public int TripId { get; set; }
    public string RiderName { get; set; }
    public string DriverName { get; set; }
    public string StartLocation { get; set; }
    public string EndLocation { get; set; }
    public decimal Fare = 25;
    public string Status { get; set; }

    public void DisplayTripDetails()
    {
        Console.WriteLine($"Trip ID: {TripId}, Rider: {RiderName}, Driver: {DriverName}, From: {StartLocation}, To: {EndLocation}, Status: {Status}, Fare: {Fare}$");
    }
}

class RideSharingSystem
{
    public List<Rider> RegisteredRiders { get; set; } = new List<Rider>();
    public List<Driver> RegisteredDrivers { get; set; } = new List<Driver>();
    public List<Trip> AvailableTrips { get; set; } = new List<Trip>();

    public void RegisterUser(User user)
    {
        user.Register();
        if (user is Rider rider)
        {
            RegisteredRiders.Add(rider);
            Console.WriteLine($"{rider.Name} registered as Rider.");
        }
        else if (user is Driver driver)
        {
            RegisteredDrivers.Add(driver);
            Console.WriteLine($"{driver.Name} registered as Driver.");
        }
    }

    public Trip RequestRide(Rider rider, string startLocation, string destination)
    {
        var trip = new Trip
        {
            TripId = AvailableTrips.Count + 1,
            RiderName = rider.Name,
            StartLocation = startLocation,
            EndLocation = destination,
            Status = "Requested"
        };
        rider.RideHistory.Add(trip);
        AvailableTrips.Add(trip);
        Console.WriteLine($"{rider.Name} requested a ride from {startLocation} to {destination}.");
        return trip;
    }

    public Driver FindAvailableDriver()
    {
        foreach (var driver in RegisteredDrivers)
        {
            if (driver.IsAvailable)
            {
                driver.ToggleAvailability();
                return driver;
            }
        }
        Console.WriteLine("No available drivers at the moment.");
        return null;
    }

    public void CompleteTrip(Trip trip)
    {
        string startLocation = trip.StartLocation;
        string destination = trip.EndLocation;
        trip.Status = "Completed";
        Console.WriteLine($"Trip {trip.TripId} from {startLocation} to {destination} completed");

    }

    public void DisplayAllTrips()
    {
        Console.WriteLine("All Trips:");
        foreach (var trip in AvailableTrips)
            trip.DisplayTripDetails();
    }
}

class Program
{
    static void Main()
    {
        var system = new RideSharingSystem();
        while (true)
        {
            Console.WriteLine("\n--- Ride Sharing System Menu ---");
            Console.WriteLine("1. Register as Rider");
            Console.WriteLine("2. Register as Driver");
            Console.WriteLine("3. Accept a Ride (Driver)");
            Console.WriteLine("4. Complete a Trip");
            Console.WriteLine("5. View Ride History (Rider/Driver)");
            Console.WriteLine("6. Display All Trips in System");
            Console.WriteLine("7. Exit");
            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.Write("Enter Rider Name: ");
                    string riderName = Console.ReadLine();
                    Console.Write("Enter Phone Number: ");
                    string riderPhone = Console.ReadLine();
                    var rider = new Rider { UserId = system.RegisteredRiders.Count + 1, Name = riderName, PhoneNumber = riderPhone };
                    system.RegisterUser(rider);

                    Console.Write("Enter Start Location: ");
                    string startLocation = Console.ReadLine();
                    Console.Write("Enter Destination: ");
                    string destination = Console.ReadLine();
                    system.RequestRide(rider, startLocation, destination);
                    break;

                case 2:
                    Console.Write("Enter Driver Name: ");
                    string driverName = Console.ReadLine();
                    Console.Write("Enter Phone Number: ");
                    string driverPhone = Console.ReadLine();
                    Console.Write("Enter Vehicle Details: ");
                    string vehicleDetails = Console.ReadLine();
                    var driver = new Driver
                    {
                        UserId = system.RegisteredDrivers.Count + 1,
                        Name = driverName,
                        PhoneNumber = driverPhone,
                        DriverID = system.RegisteredDrivers.Count + 101,
                        VehicleDetails = vehicleDetails
                    };
                    system.RegisterUser(driver);
                    break;

                case 3:
                    Driver availableDriver = system.FindAvailableDriver();
                    if (availableDriver != null && system.AvailableTrips.Count > 0)
                    {
                        Trip tripToAccept = system.AvailableTrips[0];
                        tripToAccept.DriverName = availableDriver.Name;
                        tripToAccept.Status = "In Progress";
                        availableDriver.TripHistory.Add(tripToAccept);
                        Console.WriteLine($"{availableDriver.Name} accepted the ride.");
                    }
                    else
                    {
                        Console.WriteLine("No rides available to accept.");
                    }
                    break;

                case 4:
                    if (system.AvailableTrips.Count > 0)
                    {
                        Trip tripToComplete = system.AvailableTrips[0];
                        system.CompleteTrip(tripToComplete);
                    }
                    else
                    {
                        Console.WriteLine("No trips available to complete.");
                    }
                    break;

                case 5:
                    Console.Write("Enter User ID to view history: ");
                    int userId = int.Parse(Console.ReadLine());
                    var foundRider = system.RegisteredRiders.Find(r => r.UserId == userId);
                    var foundDriver = system.RegisteredDrivers.Find(d => d.UserId == userId);

                    if (foundRider != null)
                    {
                        foundRider.DisplayProfile();
                    }
                    else if (foundDriver != null)
                    {
                        foundDriver.DisplayProfile();
                    }
                    else
                    {
                        Console.WriteLine("User not found.");
                    }
                    break;

                case 6:
                    system.DisplayAllTrips();
                    break;

                case 7:
                    Console.WriteLine("Exiting system...");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
