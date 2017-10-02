using UnityEngine;
using System.Collections;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using Contact = FarseerPhysics.Dynamics.Contacts.Contact;
using Cinch2D;

/// <summary>
/// Demonstrates the rudiments of using Farseer with Cinch2D.
/// Cinch2D uses Ian Qvist's Farseer Unity port.  For more info on Farseer (basically Box2D), check out http://farseerphysics.codeplex.com/
/// </summary>
public class Physics : DemoBase {
	
	private PhysicsContainer _physicsContainer;
	private Body _ballBody;
	private TextButton _debugDrawButton;
	
	public override void OnAwake(){
		CreateBackground();
		
		_physicsContainer = PhysicsContainer.NewEmpty(Vector2.up * -30f);
		AddChild(_physicsContainer);
		
		var field = CinchSprite.NewFromImage("Cinch2D/Field", 512);
		field.Width = ViewportWidth;
		field.Height = 1f;
		
		//create a physics body for the field, same size as the CinchSprite
		var fieldBody = BodyFactory.CreateRectangle(_physicsContainer.World, field.Width, field.Height, 1);
		
		//as soon as we set the fieldBody as field's PhysicsBody, field will take its position and rotation from fieldBody after every frame
		field.PhysicsBody = fieldBody;
		//align it with the bottom of the screen
		//setting physics body position will update sprite's position on next frame
		fieldBody.Position = new Vector2(0, 0 - ViewportHeight/2 + field.Height/2f);
		//set the field as static so it uses less CPU
		fieldBody.IsStatic = true;
		//make it a wee bit bouncy
		fieldBody.Restitution = .5f;
		_physicsContainer.AddChild(field);
		
		//create a soccer ball to roll around
		var ball = CinchSprite.NewFromImage("Cinch2D/SoccerBall", 256);
		_ballBody = BodyFactory.CreateCircle(_physicsContainer.World, .5f, 1f);
		ball.PhysicsBody = _ballBody;
		_ballBody.BodyType = BodyType.Dynamic;
		//drop it on the right side of the screen
		_ballBody.Position = new Vector2(ViewportWidth/2 - 1, 0);
		
		//we add all physics objects to the same parent.  
		//Adding a physics object as a child of another physics object (like a wheel as a child of a car), will produce super-bad results.
		//you can still add non-physics objects to different containers, like a mud splat as a child of the ball
		_physicsContainer.AddChild(ball);
		
		var player1 = CreatePlayer();
		var player2 = CreatePlayer();
		_physicsContainer.AddChild(player1);
		_physicsContainer.AddChild(player2);
		
		player1.Y = player2.Y = 0 - ViewportHeight/2 + field.Height + player1.Height/2;
		player1.X = 0 - ViewportWidth/2 + 1;
		player2.X = ViewportWidth/2 - 1;
		player1.PhysicsBody.OnCollision += BallHitPlayer1;
		player2.PhysicsBody.OnCollision += BallHitPlayer2;
		
		_debugDrawButton = Library.New<TextButton>("DebugDrawToggle");
		_debugDrawButton.Text = "Debug: Off";
		AddChild(_debugDrawButton);
		_debugDrawButton.Y = 0 + ViewportHeight / 2 - _debugDrawButton.Height;
		//toggle debug draw when pressed
		_debugDrawButton.AddEventListener<MouseEvent>(MouseEvent.MOUSE_UP, ToggleDebugDraw);
	}
	
	private CinchSprite CreatePlayer(){
		var player = CinchSprite.NewFromSpriteSheet("Cinch2D/SoccerPlayer", 0, 0, 160, 256, 128);
		player.PhysicsBody = BodyFactory.CreateRectangle(_physicsContainer.World, 1f, 2f, 1f);
		player.PhysicsBody.BodyType = BodyType.Static;
		player.PhysicsBody.IsSensor = true;
		return player;
	}

	bool BallHitPlayer1 (Fixture fixtureA, Fixture fixtureB, Contact contact){
		//fire it right and up
		_ballBody.LinearVelocity = new Vector2(Random.Range (ViewportWidth * .5f, ViewportWidth * .8f), Random.Range (6f, 9f));
		return true;
	}

	bool BallHitPlayer2 (Fixture fixtureA, Fixture fixtureB, Contact contact){
		//fire it left and up
		_ballBody.LinearVelocity = new Vector2(Random.Range (ViewportWidth * -.8f, ViewportWidth * -.5f), Random.Range (6f, 9f));
		return true;
	}
	
	private void ToggleDebugDraw(MouseEvent me)
	{
		_physicsContainer.DebugDrawEnabled = !_physicsContainer.DebugDrawEnabled;
		_debugDrawButton.Text = _physicsContainer.DebugDrawEnabled ? "Debug: On" : "Debug: Off";
	}
}
