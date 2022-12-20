using TransactionPattern.Application.UseCases.ResgisterResource;
using TransactionPattern.Infrastructure;

Console.WriteLine("Hello, World!");

var usecase = new RegisterResourceUseCase(new HttpClient(), new FileInfo("test.json"), new DataRepository());
Console.WriteLine(usecase.Execute());