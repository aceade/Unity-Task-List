using System;
using System.Collections.Generic;

/// <summary>
/// A task that needs to be done.
/// </summary>

[System.Serializable]
public class Task
{
	public DateTime startDate;

	public DateTime endDate;

	public string name;

	public string description;

	public enum status
	{
		Open,
		Solved
	}

	public status currentStatus;

	/// <summary>
	/// Initialises a new instance of the <see cref="Task"/> class.
	/// </summary>
	/// <param name="newName">The name of the task.</param>
	/// <param name="newDesc">A description.</param>
	public Task(string newName, string newDesc)
	{
		this.name = newName;
		this.description = newDesc;
		this.startDate = DateTime.Now;
		this.currentStatus = status.Open;
	}

	/// <summary>
	/// Initialises a new instance of the <see cref="Task"/> class.
	/// </summary>
	/// <param name="newName">The name of the task.</param>
	public Task (string newName)
	{
		this.name = newName;
		this.description = "Describe the task here";
		this.startDate = DateTime.Now;
		this.currentStatus = status.Open;
	}

	/// <summary>
	/// Initialises a new default instance of the <see cref="Task"/> class.
	/// </summary>
	public Task()
	{
		this.name = "New Task";
		this.description = "Describe the task here";
		this.startDate = DateTime.Now;
		this.currentStatus = status.Open;
	}

	public void Complete()
	{
		if (this.currentStatus == status.Open)
		{
			this.currentStatus = status.Solved;
			this.endDate = DateTime.Now;
		}
	}

}
