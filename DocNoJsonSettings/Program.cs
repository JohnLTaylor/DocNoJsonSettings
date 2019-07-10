using CosmosDbRepository;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace DocNoJsonSettings
{
    class Program
    {
        private const string AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private static Uri serviceEndpoint = new Uri("https://localhost:8081");

        static async Task Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK",
                DateParseHandling = DateParseHandling.DateTimeOffset,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,

            };

            (var src1, var dst1) = await RunTest(null);
            dst1.Should().BeEquivalentTo(src1);
            dst1.LocalTime.Offset.Should().Be(src1.LocalTime.Offset);
            dst1.UtcTime.Offset.Should().NotBe(src1.UtcTime.Offset);

            (var src2, var dst2) = await RunTest(JsonConvert.DefaultSettings());
            dst2.Should().BeEquivalentTo(src2);
            dst2.LocalTime.Offset.Should().Be(src2.LocalTime.Offset);
            dst2.UtcTime.Offset.Should().Be(src2.UtcTime.Offset);
        }

        private static async Task<(TypedDocument source, TypedDocument destination)> RunTest(JsonSerializerSettings serializerSettings)
        {
            IDocumentClient client = new DocumentClient(serviceEndpoint, AuthKey, serializerSettings);
            var db = new CosmosDbBuilder()
                .WithId("Foo")
                .AddCollection<TypedDocument>("Bar")
                .AddCollection<Document>("Bar")
                .Build(client);

            var repo = db.Repository<TypedDocument>();

            var now = DateTimeOffset.Now;
            var source = new TypedDocument
            {
                Id = Guid.NewGuid(),
                LocalTime = now,
                UtcTime = now.ToUniversalTime()
            };

            await repo.AddAsync(source);
            var destination = await repo.GetAsync(source.Id);

            return (source, destination);
        }
    }
}
