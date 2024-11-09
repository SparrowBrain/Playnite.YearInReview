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
		public void Compose_AssignsMostPlayedGame(
			[Frozen] IMostPlayedGameAggregator mostPlayedGameAggregatorFake,
			Metadata metadata,
			Game mostPlayedGame,
			int year,
			List<Activity> activities,
			Composer1970 sut)
		{
			// Arrange
			A.CallTo(() => mostPlayedGameAggregatorFake.GetMostPlayedGame(A<IReadOnlyCollection<Activity>>._)).Returns(mostPlayedGame);

			// Act
			var result = sut.Compose(year, activities);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(mostPlayedGame.Id, result.MostPlayedGame.Id);
			Assert.Equal(mostPlayedGame.Name, result.MostPlayedGame.Name);
			Assert.Equal(mostPlayedGame.CoverImage, result.MostPlayedGame.CoverImage);
		}
	}

	public class Composer1970
	{
		private readonly IMetadataProvider _metadataProvider;
		private readonly IMostPlayedGameAggregator _mostPlayedGameAggregator;

		public Composer1970(
			IMetadataProvider metadataProvider,
			IMostPlayedGameAggregator mostPlayedGameAggregator)
		{
			_metadataProvider = metadataProvider;
			_mostPlayedGameAggregator = mostPlayedGameAggregator;
		}

		public Report1970 Compose(int year, IReadOnlyCollection<Activity> activities)
		{
			var mostPlayedGame = _mostPlayedGameAggregator.GetMostPlayedGame(activities);

			return new Report1970()
			{
				Metadata = _metadataProvider.Get(year),

				MostPlayedGame = new MostPlayedGame()
				{
					Id = mostPlayedGame.Id,
					Name = mostPlayedGame.Name,
					CoverImage = mostPlayedGame.CoverImage,
					FlavourText = "Most played game of the year"
				}
			};
		}
	}
}