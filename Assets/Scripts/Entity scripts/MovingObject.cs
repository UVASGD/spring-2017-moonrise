using UnityEngine;
using System.Collections;

namespace Completed
{


    public abstract class MovingObject : MonoBehaviour
    {
		public float speed = 1f;			//object speed, added to the object's AP each tick
		public float AP = 0f;				//Action Points, every action for now costs 1 AP
		
        public float moveTime = 0.1f;		//Movement time for animation
        public LayerMask blockingLayer;		

        private BoxCollider2D boxCollider;	//boxCollider for the object - used for raycast tests?
        private Rigidbody2D rb2D;			
        private float inverseMoveTime;		// 1/moveTime, used to calculate the distance per frame for movement

		public Orientation orientation;

        // Use this for initialization
        protected virtual void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            rb2D = GetComponent<Rigidbody2D>();
            inverseMoveTime = 1f / moveTime;
        }
        protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
			//Find movement points
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);
	
			//Check if the move isn't blocked
            boxCollider.enabled = false;
            hit = Physics2D.Linecast(start, end, blockingLayer);
            boxCollider.enabled = true;

			//If the move is not blocked, move with animation
            if (hit.transform == null)
            {
                StartCoroutine(SmoothMovement(end));
                return true;
            }
            return false;
        }
		
		//Don't bother with this for now, it moves objects from start to stop in moveTime seconds
        protected IEnumerator SmoothMovement(Vector3 end)
        {
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            while (sqrRemainingDistance > float.Epsilon)
            {
				Vector3 newPosition = end;//Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
                rb2D.MovePosition(newPosition);
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                yield return null;
            }

			OnFinishMove();
        }

		//Similar to move, but if a move fails due to a unit, strike the unit.
        protected virtual void AttemptMove(int xDir, int yDir)
        {
			RaycastHit2D hit;
			bool canMove = Move(xDir, yDir, out hit);
			if (xDir > 0)
				orientation = Orientation.East;
			else if (xDir < 0)
				orientation = Orientation.West;
			else if (yDir > 0)
				orientation = Orientation.North;
			else
				orientation = Orientation.South;
			UpdateSprite ();

        }

        protected abstract void OnCantMove(Transform transform); // expects transform to be not null
		protected abstract void OnFinishMove();
		protected abstract void UpdateSprite();
    }
}
