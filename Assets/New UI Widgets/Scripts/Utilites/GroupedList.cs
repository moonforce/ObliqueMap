namespace UIWidgets
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Base class for GroupedList.
	/// </summary>
	/// <typeparam name="T">Items type.</typeparam>
	public abstract class GroupedList<T>
	{
		/// <summary>
		/// Contains groups and items for each group.
		/// Group as key. Items for group as value.
		/// </summary>
		protected Dictionary<T, List<T>> GroupsWithItems = new Dictionary<T, List<T>>();

		/// <summary>
		/// Group comparison.
		/// </summary>
		public Comparison<T> GroupComparison;

		/// <summary>
		/// Dict converted to flat list.
		/// </summary>
		public ObservableList<T> Data;

		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		protected abstract T GetGroup(T item);

		/// <summary>
		/// Add item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void Add(T item)
		{
			var group = GetGroup(item);
			if (!GroupsWithItems.ContainsKey(group))
			{
				GroupsWithItems.Add(group, new List<T>());
			}

			GroupsWithItems[group].Add(item);

			Update();
		}

		/// <summary>
		/// Remove item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item exists and deleted; otherwise, false.</returns>
		public bool Remove(T item)
		{
			var group = GetGroup(item);
			if (!GroupsWithItems.ContainsKey(group))
			{
				return false;
			}

			GroupsWithItems[group].Remove(item);

			if (GroupsWithItems[group].Count == 0)
			{
				GroupsWithItems.Remove(group);
			}

			Update();

			return true;
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			GroupsWithItems.Clear();
			Update();
		}

		/// <summary>
		/// Update data list.
		/// </summary>
		public void Update()
		{
			if (inUpdate)
			{
				isChanged = true;
				return;
			}

			if (Data == null)
			{
				return;
			}

			Data.BeginUpdate();

			Data.Clear();

			var groups = new List<T>(GroupsWithItems.Keys);
			if (GroupComparison != null)
			{
				groups.Sort(GroupComparison);
			}

			groups.ForEach(group =>
			{
				Data.Add(group);
				Data.AddRange(GroupsWithItems[group]);
			});

			Data.EndUpdate();
		}

		bool inUpdate;
		bool isChanged;

		/// <summary>
		/// Pause data list update.
		/// </summary>
		public void BeginUpdate()
		{
			inUpdate = true;
		}

		/// <summary>
		/// Unpause data list update.
		/// </summary>
		public void EndUpdate()
		{
			inUpdate = false;
			if (isChanged)
			{
				isChanged = false;
				Update();
			}
		}
	}
}