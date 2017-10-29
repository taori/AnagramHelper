using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anagram.Client.Properties;

namespace Anagram.Client
{
	public class MainViewModel : ViewModelBase
	{
		public MainViewModel()
		{
			Sample = Settings.Default.LastSample;
			AnagramTest = Settings.Default.LastTestAnagram;
		}

		private readonly Dictionary<char, int> AvailableCount = new Dictionary<char, int>();
		private readonly Dictionary<char, int> UsedCount = new Dictionary<char, int>();
		private readonly Dictionary<char, int> CountDifference = new Dictionary<char, int>();

		private void UpdateState()
		{
			AvailableCount.Clear();
			UsedCount.Clear();
			CountDifference.Clear();

			var availableChars = Sample?.ToLower()?.OrderBy(d => d)?.ToArray() ?? new char[0];
			var usedChars = AnagramTest?.ToLower()?.OrderBy(d => d)?.ToArray() ?? new char[0];

			foreach (var c in availableChars)
			{
				if (AvailableCount.TryGetValue(c, out var v))
				{
					AvailableCount[c] = (v + 1);
				}
				else
				{
					AvailableCount.Add(c, 1);
				}
			}

			foreach (var c in usedChars)
			{
				if (UsedCount.TryGetValue(c, out var v))
				{
					UsedCount[c] = (v + 1);
				}
				else
				{
					UsedCount.Add(c, 1);
				}
			}

			var anyUsedChar = new HashSet<char>(usedChars.Concat(availableChars));
			var anySortedChar = anyUsedChar?.OrderBy(d => d)?.ToArray() ?? new char[0];

			foreach (var c in anyUsedChar)
			{
				if (!AvailableCount.TryGetValue(c, out var availableCount))
					availableCount = 0;
				if (!UsedCount.TryGetValue(c, out var usedCount))
					usedCount = 0;

				CountDifference.Add(c, availableCount - usedCount);
			}

			var anyDifference = false;
			var remainingSb = new StringBuilder();
			var surplusSb = new StringBuilder();
			
			foreach (var c in anySortedChar)
			{
				if (CountDifference.TryGetValue(c, out var dif))
				{
					if(dif == 0)
						continue;

					anyDifference = true;

					if (dif > 0)
					{
						remainingSb.Append(c, dif);
						remainingSb.Append(' ');
					}
					else
					{
						surplusSb.Append(c, Math.Abs(dif));
						surplusSb.Append(' ');
					}
				}
			}

			SurplusChars = surplusSb.ToString()?.Trim();
			RemainingChars = remainingSb.ToString()?.Trim();

			IsAnagram = !anyDifference;
		}

		private string _sample;

		public string Sample
		{
			get { return _sample; }
			set
			{
				if (value == _sample)
					return;
				_sample = value;
				OnPropertyChanged();

				Settings.Default.LastSample = value;
				Settings.Default.Save();

				UpdateState();
			}
		}

		private string _anagramTest;

		public string AnagramTest
		{
			get { return _anagramTest; }
			set
			{
				if (value == _anagramTest)
					return;
				_anagramTest = value;
				OnPropertyChanged();

				Settings.Default.LastTestAnagram = value;
				Settings.Default.Save();

				UpdateState();
			}
		}

		private string _remainingChars;

		public string RemainingChars
		{
			get { return _remainingChars; }
			set
			{
				if (value == _remainingChars)
					return;
				_remainingChars = value;
				OnPropertyChanged();
			}
		}

		private string _surplusChars;

		public string SurplusChars
		{
			get { return _surplusChars; }
			set
			{
				if (value == _surplusChars)
					return;
				_surplusChars = value;
				OnPropertyChanged();
			}
		}

		private bool _isAnagram;

		public bool IsAnagram
		{
			get { return _isAnagram; }
			set
			{
				if (value == _isAnagram)
					return;
				_isAnagram = value;
				OnPropertyChanged();
			}
		}
	}
}