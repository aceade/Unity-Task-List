using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

/// <summary>
/// Todo list panel.
/// </summary>

public class TodoListPanel : EditorWindow 
{
	// the list of categorys
	public static List<Category> categoryList = new List<Category>();
	static Category emptyCategory = new Category("Uncategorised");

	static List<string> categoryNames = new List<string>();

	static List<bool> toggleCategory = new List<bool>();

	static List<bool> toggleTasks = new List<bool>();

	private bool creatingTask = false;

	private bool creatingCategory = false;

	private static string newCatName = "New Category";
	private static string newTaskName = "New Task";
	
	private Vector2 scrollPos = Vector2.zero;

	int catIndex;			// need an index to choose categories

	/// <summary>
	/// The data path is the root of the assets folder.
	/// </summary>
	private static string dataPath = Application.dataPath + "/Aceade/Todo List";

	[MenuItem ("Window/Todo List")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(TodoListPanel));
	}

	static TodoListPanel()
	{
		LoadCategories();
	}
	
	void OnGUI()
	{

		GUILayout.Label("Todo List");
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();

		if (GUILayout.Button("New Category") )
		{
			creatingCategory = !creatingCategory;
			creatingTask = false;
		}

		if (GUILayout.Button("Save Categories") )
		{
			// save to the disk
			SaveCategories();
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical();

		if (GUILayout.Button("New Task") )
		{
			// create a new task
			creatingTask = !creatingTask;
			creatingCategory = false;

		}
		if (GUILayout.Button("Load") )
		{
			// load
			LoadCategories();
		}

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// create new Tasks and Categories here
		GUILayout.BeginHorizontal();
		if (creatingTask)
		{
			// get the new name from a text field
			newTaskName = GUILayout.TextField(newTaskName, GUILayout.MinWidth(200) );

			// use a Popup window here for category
			catIndex = EditorGUILayout.Popup(catIndex, categoryNames.ToArray());

			if (GUILayout.Button("Add Task") )
			{
				Task newTask = new Task(newTaskName);
				categoryList[catIndex].AddTask (newTask);
				toggleTasks.Add(false);
				newTaskName = "New Task";
				creatingTask = false;
			}
		}
		if (creatingCategory)
		{
			newCatName = GUILayout.TextField(newCatName, GUILayout.MinWidth(200) );

			if (GUILayout.Button("Add Category") )
			{
				Category newCat = new Category(newCatName);
				if (categoryList.Count == 0)
				{
					emptyCategory = newCat;
				}
				categoryList.Add (newCat);
				categoryNames.Add (newCat.name);
				toggleCategory.Add (false);
				newCatName = "New Category";
				creatingCategory = false;
			}
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		if (categoryList.Count > 0 && toggleCategory.Count > 0)
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			for (int i = 0; i < categoryList.Count; i++)
			{
				Category thisCat = categoryList[i];
				toggleCategory[i] = EditorGUILayout.Foldout(toggleCategory[i], thisCat.name);

				if (toggleCategory[i] == true)
				{

					GUILayout.BeginHorizontal();
					thisCat.name = GUILayout.TextField(thisCat.name, GUILayout.MinWidth(200) );
					if (GUILayout.Button("Remove Category"))
					{
						toggleCategory.RemoveAt(categoryList.IndexOf(thisCat) );
						categoryList.Remove(thisCat);
						categoryNames.Remove(thisCat.name);

					}

					GUILayout.EndHorizontal();
					EditorGUILayout.Space();

					// loop through the tasks of the category
					if (thisCat.categoryTasks.Count > 0)
					{
						for (int j = 0; j< thisCat.categoryTasks.Count; j++)
						{

							// for the current task, display it's name, options to delete or finish the task
							Task thisTask = thisCat.categoryTasks[j];
							thisTask.name = GUILayout.TextField(thisTask.name, GUILayout.MinWidth(200) );

							GUILayout.BeginHorizontal();
							if (GUILayout.Button("Delete Task") )
							{
								thisCat.RemoveTask(thisTask);
							}
							if (thisTask.currentStatus == Task.status.Open)
							{
								if (GUILayout.Button("Finished") )
									thisTask.Complete();
							}

							GUILayout.EndHorizontal();

							// display the start and end date
							GUILayout.BeginHorizontal();
							GUILayout.Box("Started " + thisTask.startDate.ToShortDateString() );
							if (thisTask.currentStatus == Task.status.Solved)
							{
								GUILayout.Box("Finished " + thisTask.endDate.ToShortDateString() );
							}
							else
							{
								GUILayout.Box("Status: " + thisTask.currentStatus.ToString() );
							}

							GUILayout.EndHorizontal();

							// allow the user to update the description
							thisTask.description = GUILayout.TextArea(thisTask.description);

							EditorGUILayout.Space();

						}
					}

					EditorGUILayout.Space();

				}
			}
			EditorGUILayout.EndScrollView();
		}

	}	

	void OnInspectorUpdate()
	{
		this.Repaint();
	}

	/// <summary>
	/// Saves the categories.
	/// </summary>
	void SaveCategories()
	{
		BinaryFormatter bf = new BinaryFormatter();
		var saveFile = File.Create(dataPath + "/tasks.dat");
		bf.Serialize(saveFile, categoryList);
		saveFile.Close();
	}

	/// <summary>
	/// Loads the categories.
	/// </summary>
	static void LoadCategories()
	{
		try 
		{
			// check if the file exists before doing anything.
			if (File.Exists(dataPath + "/tasks.dat"))
			{
				BinaryFormatter bf = new BinaryFormatter();
				var f = File.Open(dataPath + "/tasks.dat",FileMode.OpenOrCreate );
				categoryList = (List<Category>)bf.Deserialize(f);
				emptyCategory = categoryList[0];
				toggleCategory.Clear();
				for (int i = 0 ; i < categoryList.Count; i++)
				{
					toggleCategory.Add (false);
					categoryNames.Add (categoryList[i].name);
				}
			}

		}
		catch (SerializationException e)
		{
			Debug.LogErrorFormat("Error loading categories: {0}", e);
		}

	}

	void OnApplicationQuit()
	{
		Debug.Log("Quitting. " + categoryList.Count + ", " + toggleCategory.Count + ", " + categoryNames.Count);
		categoryList.Clear();
		categoryNames.Clear();
		toggleCategory.Clear();
	}

}