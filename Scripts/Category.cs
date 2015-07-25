using System;
using System.Collections.Generic;

/// <summary>
/// A Category is a collection of tasks.
/// </summary>

[System.Serializable]
public class Category
{
	public List<Task> categoryTasks;

	public string name;

	public Category(string newName)
	{
		this.name = newName;
		this.categoryTasks = new List<Task>();
	}

	public void AddTask(Task newTask)
	{
		this.categoryTasks.Add (newTask);
	}

	public void RemoveTask(Task taskToRemove)
	{
		this.categoryTasks.Remove(taskToRemove);
	}
}
