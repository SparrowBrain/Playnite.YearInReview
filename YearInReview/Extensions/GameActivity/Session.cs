using System;
using System.Collections.Generic;

namespace YearInReview.Extensions.GameActivity
{
	public class Session : ObservableObject
	{
		private Guid _sourceId = default;
		private Guid _platformId = default;
		private List<Guid> _platformIDs = new List<Guid>();
		private int _idConfiguration = 0;
		private DateTime _dateSession = default;
		private ulong _elapsedSeconds = 0;

		public Guid SourceId { get => _sourceId; set => SetValue(ref _sourceId, value); }
		public Guid PlatfromId { get => _platformId; set => SetValue(ref _platformId, value); }
		public List<Guid> PlatformIDs { get => _platformIDs; set => SetValue(ref _platformIDs, value); }
		public int IdConfiguration { get => _idConfiguration; set => SetValue(ref _idConfiguration, value); }
		public DateTime DateSession { get => _dateSession; set => SetValue(ref _dateSession, value); }
		public ulong ElapsedSeconds { get => _elapsedSeconds; set => SetValue(ref _elapsedSeconds, value); }
	}
}