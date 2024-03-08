// See https://aka.ms/new-console-template for more information

using PatientClient;

Console.WriteLine("count:");
var sender = new ApiSender("http://localhost:3500");
var generator = new PatientGenerator();
while (true)
{
    var key = Console.ReadLine();
    int count;
    if (key != null && int.TryParse(key, out count))
    {
        var patients = generator.GenerateList(count);
        var result = await sender.CreateMany(patients);
        Console.WriteLine(result);
        Console.ReadLine();
        break;
    }
    else
    {
        Console.WriteLine("incorrect value");
        continue;
    }
}
