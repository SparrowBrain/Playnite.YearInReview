using AutoFixture.Xunit2;
using FakeItEasy;
using Playnite.SDK.Models;
using System.Collections.Generic;
using TestTools.Shared;
using Xunit;
using YearInReview.Extensions.GameActivity;
using YearInReview.Model.Aggregators;
using YearInReview.Model.Aggregators.Data;
using YearInReview.Model.Reports._1970;

namespace YearInReview.UnitTests.Model.Reports._1970
{
	public class Composer1970Tests
	{
		[Theory]
		[AutoFakeItEasyData]
		public void Compose_AssignsMetadata(
			[Frozen] IMetadataProvider metadataProviderFake,
			Metadata metadata,
			Game mostPlayedGame,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => metadataProviderFake.Get(A<int>._)).Returns(metadata);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(metadata, result.Metadata);
		}

		[Theory]
		[AutoFakeItEasyData]
		public void Compose_AssignsMostPlayedGames(
			[Frozen] IMostPlayedGamesAggregator mostPlayedGameAggregatorFake,
			Metadata metadata,
			List<GameWithTime> mostPlayedGames,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => mostPlayedGameAggregatorFake.GetMostPlayedGame(A<IReadOnlyCollection<Activity>>._, A<int>._)).Returns(mostPlayedGames);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(mostPlayedGames.Count, result.MostPlayedGames.Count);
		}
	}
}