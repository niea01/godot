using Godot;
using System;
using System.Collections.Generic;

public class sliding : Node2D
{
	private Sprite[,] imgMap = new Sprite[4, 4];

	private Vector2 imgPos = new Vector2(3, 3);
	private Vector2 move = new Vector2(0,0);
	private int speed = 1;

	private Label msgbox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.msgbox = GetNode<Label>("Label");
		this.msgbox.Hide();
		
		var imgNode = GD.Load("res://img.tscn") as PackedScene;

		for (var x = 0; x < 4; x++)
		{
			for (var y = 0; y < 4; y++)
			{
				if (this.imgPos.x == x && this.imgPos.y == y) continue;

				var img = imgNode.Instance() as Sprite;
				img.Position = new Vector2(x * 16, y *16);
				img.Frame = x + (y * 4);
				img.ZIndex = -1;

				this.imgMap[x, y] = img;
				this.AddChild(img);
			}
		}
		
		//shuffle
		var rnd = new Random();
		for (var i = 0; i < 30; i++)
		{
			switch(rnd.Next(4))
			{
			case 0:
				moveImg(new Vector2(1, 0), true);
				break;
			case 1:
				moveImg(new Vector2(-1, 0), true);
				break;
			case 2:
				moveImg(new Vector2(0, 1), true);
				break;
			case 3:
				moveImg(new Vector2(0, -1), true);
				break;	
			}
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		step("ui_left", new Vector2(1, 0));
		step("ui_right", new Vector2(-1, 0));
		step("ui_up", new Vector2(0, 1));
		step("ui_down", new Vector2(0, -1));
	}

	private void step(string key, Vector2 dir)
	{
		if(Input.IsActionPressed(key) && move == new Vector2(0,0))
		{
			if (!moveImg(dir, false)) return;
			this.move = dir;
			//GD.Print("Move:pos=" + this.imgPos + " key=" + key);
		}

		if (move == dir)
		{
			var img = this.imgMap[(int)this.imgPos.x, (int)this.imgPos.y];
			img.Position += new Vector2(dir.x * speed * -1, dir.y * speed * -1);

			if (img.Position == new Vector2(this.imgPos.x * 16, this.imgPos.y * 16))
			{
				this.imgPos += dir;
				this.move = new Vector2(0, 0);
				
				if (isComplete())
				{
					msgbox.Show();
				}
				else
				{
					msgbox.Hide();
				}
			}
		}
	}
	
	private bool moveImg(Vector2 dir, bool changePos)
	{
		var pos = this.imgPos + dir;
		pos.x = Math.Min(Math.Max(pos.x, 0), 3);
		pos.y = Math.Min(Math.Max(pos.y, 0), 3);
		if (pos == this.imgPos) return false;

		var img = this.imgMap[(int)pos.x, (int)pos.y];
		this.imgMap[(int)pos.x, (int)pos.y] = null;
		this.imgMap[(int)this.imgPos.x, (int)this.imgPos.y] = img;
		
		if (changePos)
		{
			img.Position = new Vector2(this.imgPos.x * 16, this.imgPos.y * 16);
			this.imgPos += dir;
		}
		
		return true;
	}
	
	private bool isComplete()
	{
		for (var x = 0; x < 4; x++)
		{
			for (var y = 0; y < 4; y++)
			{
				var img = this.imgMap[x, y];
				if (img != null && img.Frame != x + (y * 4))
				{
					return false;	
				}
			}
		}
		return true;
	}
}
