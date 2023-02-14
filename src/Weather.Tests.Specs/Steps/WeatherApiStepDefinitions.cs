using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using Weather.Domain;
using Weather.Domain.Utils;
using Weather.Specs.Mocks;
using Xunit;

namespace Weather.Specs.Steps;

[Binding]
public sealed class WeatherApiStepDefinitions
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef
    private readonly ScenarioContext _scenarioContext;
    private int _unixTime;
    private DateTime _result;
    private List<string> _towns;
    private readonly WeatherApi _weatherApi;

    public WeatherApiStepDefinitions(ScenarioContext scenarioContext, HttpClient httpClient)
    {
        _scenarioContext = scenarioContext;
        _weatherApi = new WeatherApi(httpClient);
    }

    [Given(@"the test datetime in unix format (.*)")]
    public void GivenTheFirstNumberIs(int number)
    {
        _unixTime = number;
    }

    [When("app converted it from unix to datetime format")]
    public void WhenTheTwoNumbersAreAdded()
    {
        _result = DateTimeUtils.UnixTimeStampToDateTime(_unixTime);
    }

    [Then("the result should be (.*)")]
    public void ThenTheResultShouldBe(DateTime result)
    {
        Assert.Equal(_result, result);
    }

    [Given(@"I have locations with names")]
    public void GivenIHaveLocationsWithNames(Table table)
    {
        _towns = table.Rows.Select(t => t["Name"]).ToList();
    }

    [When(@"I'm requesting weather")]
    public async Task WhenImRequestingWeather()
    {
        foreach (var town in _towns)
        {
            var temp = await _weatherApi.GetWeatherByCityName(town);
            _scenarioContext["temps"] += temp;
        }
    }

    [Then(@"the system should return following temperatures")]
    public void ThenTheSystemShouldReturnFollowingTemperatures(Table table)
    {
        _scenarioContext["temps"].Should().Equals(table.CreateSet<double>());
    }
}