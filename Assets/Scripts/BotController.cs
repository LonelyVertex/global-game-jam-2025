using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotController : MonoBehaviour
{
    readonly Color DebugColor = Color.cyan;

    public PlayerController pc;
    public LayerMask collisionLayer;
    [Space]
    public float checkDistance;

    bool _rotationChosen;
    Vector4 _rotation;
    
    Transform _closestWeaponPosition;

    void Update()
    {
        if (pc.killed) return;
        
        if (HasWeapon())
        {
            Log("Can shoot, looking for players");
            
            UpdateShooting();
        }
        else
        {
            UpdateWeaponSearch();
        }
    }

    void UpdateWeaponSearch()
    {
        Log("Update Weapon Search");
        
        var collectibles = FindObjectsByType<CollectibleWeapon>(FindObjectsSortMode.None);
        
        if (collectibles.Length == 0)
        {
            Log("No collectibles found, hide and fallback to driving");
            
            pc.SetUnderwater(true);
            
            UpdateDriving();
            return;
        }
        
        pc.SetUnderwater(false);
        
        var closestWeapon = collectibles.OrderBy(collectible => Vector2.Distance(pc.transform.position, collectible.transform.position)).First();
        Debug.DrawLine(pc.transform.position, closestWeapon.transform.position, DebugColor);
        
        var closestWeaponDistance = Vector2.Distance(pc.transform.position, closestWeapon.transform.position);
        var forwardDistance = CollisionDistance(pc.transform.up);
        
        if (forwardDistance < closestWeaponDistance)
        {
            Debug.Log("Can't reach weapon, fallback to driving");
            
            UpdateDriving();
            return;
        }
        
        var direction = (closestWeapon.transform.position - pc.transform.position).normalized;
        
        // Cross product to find perpendicular vector
        var cross = Vector3.Cross(pc.transform.up, direction);

        // Dot product with the up vector (Y-axis) to check direction
        var dot = Vector3.Dot(cross, -Vector3.forward);

        var inputVector = new Vector4(1, 0, 0, 0);
        
        if (dot > 0)
        {
            Debug.Log("Turning right");
            inputVector += new Vector4(0, 0, 0, 1);
        }
        else if (dot < 0)
        {
            Debug.Log("Turning left");
            inputVector += new Vector4(0, 0, 1, 0);
        }
        else
        {
            Debug.Log("Moving towards weapon");
        }
        
        
        pc.SetInputVector(inputVector);
        
        _rotationChosen = false;
    }

    void UpdateShooting()
    {
        var closestPlayer = FindOtherPlayers().OrderBy(other => Vector2.Distance(pc.transform.position, other.transform.position)).First();

        if (!closestPlayer)
        {
            Debug.Log("No players found, fallback to driving");
            UpdateDriving();
            return;
        }
        
        Debug.DrawLine(pc.transform.position, closestPlayer.transform.position, DebugColor);
        
        var closestWeaponDistance = Vector2.Distance(pc.transform.position, closestPlayer.transform.position);
        var forwardDistance = CollisionDistance(pc.transform.up);
        
        if (forwardDistance < closestWeaponDistance)
        {
            Debug.Log("Can't reach other player, fallback to driving");
            
            UpdateDriving();
            return;
        }
        
        var direction = (closestPlayer.transform.position - pc.transform.position).normalized;
        
        // Cross product to find perpendicular vector
        var cross = Vector3.Cross(pc.transform.up, direction);

        // Dot product with the up vector (Y-axis) to check direction
        var dot = Vector3.Dot(cross, -Vector3.forward);

        var inputVector = new Vector4(1, 0, 0, 0);
        
        if (dot > 0)
        {
            Debug.Log("Turning right");
            inputVector += new Vector4(0, 0, 0, 1);
        }
        else if (dot < 0)
        {
            Debug.Log("Turning left");
            inputVector += new Vector4(0, 0, 1, 0);
        }
        else
        {
            Debug.Log("Moving towards player");
        }
        
        TryShoot();
        
        pc.SetInputVector(inputVector);
        
        _rotationChosen = false;
    }

    void UpdateDriving()
    {
        var inputVector = new Vector4(1, 0, 0, 0);

        var forwardDistance = CollisionDistance(pc.transform.up);

        if (forwardDistance < Mathf.Infinity)
        {
            if (!_rotationChosen)
            {
                var leftDistance = LeftCollisionDistance();
                var rightDistance = RightCollisionDistance();

                if (leftDistance < rightDistance)
                {
                    _rotation = new Vector4(0, 0, 1, 0);
                }
                else
                {
                    _rotation = new Vector4(0, 0, 0, 1);
                }

                _rotationChosen = true;
            }
            
            inputVector += _rotation;
        }
        else
        {
            _rotationChosen = false;
        }

        pc.SetInputVector(inputVector);
    }

    void TryShoot()
    {
        if (!ShouldShoot()) return;
        
        pc.Fire();
    }

    float CollisionDistance(Vector3 direction)
    {
        Debug.DrawLine(pc.transform.position, pc.transform.position + direction * checkDistance, DebugColor);

        var hits = Physics2D.RaycastAll(pc.transform.position, direction, checkDistance, collisionLayer);
        var result = hits.Where(hit => hit.collider != null && hit.collider.gameObject.CompareTag("Obstacle"))
            .Select(hit => Vector2.Distance(pc.transform.position, hit.point))
            .OrderBy(point => point)
            .FirstOrDefault();

        return result == 0 ? Mathf.Infinity : result;
    }

    float RightCollisionDistance()
    {
        var rotation = Quaternion.Euler(0, 0, 30);
        var direction = rotation * pc.transform.up;
        return CollisionDistance(direction);
    }

    float LeftCollisionDistance()
    {
        var rotation = Quaternion.Euler(0, 0, -30);
        var direction = rotation * pc.transform.up;
        return CollisionDistance(direction);
    }

    bool HasWeapon()
    {
        return pc.currentWeapon != PlayerController.WeaponType.Hands;
    }

    bool ShouldShoot()
    {
        var players = FindOtherPlayers();
        var playersInSight = players.Any(player => FindAngle(player.transform) < 30);

        if (playersInSight && pc.currentWeapon == PlayerController.WeaponType.RocketLauncher)
        {
            var collisionDistance = CollisionDistance(pc.transform.up);
            
            return collisionDistance > 4;
        }

        return playersInSight;
    }

    float FindAngle(Transform otherPlayer)
    {
        var direction = otherPlayer.position - pc.transform.position;
        
        var angle = Vector2.SignedAngle(pc.transform.up, direction);
        
        Debug.Log($"Angle: {otherPlayer.name} - {angle}");
        
        return Vector2.SignedAngle(pc.transform.up, direction);
    }

    IEnumerable<PlayerController> FindOtherPlayers()
    {
        return FindObjectsByType<PlayerController>(FindObjectsSortMode.None).Where(otherPc => pc != otherPc && !pc.killed);
    }
    
    void Log(string msg)
    {
        Debug.Log($"[BOT][{gameObject.name}] {msg}");
    }
}