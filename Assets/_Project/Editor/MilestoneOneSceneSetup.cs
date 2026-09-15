using TreasureDivers.Interaction;
using TreasureDivers.Player;
using TreasureDivers.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TreasureDivers.EditorTools
{
    public static class MilestoneOneSceneSetup
    {
        private const string PrototypeScenePath = "Assets/_Project/Scenes/Prototype_DiveRoom.unity";

        [MenuItem("Tools/Treasure Divers/Milestone 1/Configure Prototype Dive Room")]
        public static void ConfigurePrototypeDiveRoom()
        {
            if (!OpenPrototypeSceneIfNeeded())
            {
                return;
            }

            GameObject player = FindRequiredObject("Player");
            GameObject treasure = FindRequiredObject("TestTreasure");

            CharacterController controller = EnsureComponent<CharacterController>(player);
            Undo.RecordObject(controller, "Configure Player Character Controller");
            controller.height = 1.8f;
            controller.radius = 0.35f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            controller.stepOffset = 0.35f;
            controller.slopeLimit = 50f;

            DiverInputReader inputReader = EnsureComponent<DiverInputReader>(player);
            WaterSensor waterSensor = EnsureComponent<WaterSensor>(player);
            FirstPersonDiverMotor motor = EnsureComponent<FirstPersonDiverMotor>(player);
            DiverCursorLock cursorLock = EnsureComponent<DiverCursorLock>(player);
            PlayerInteractor interactor = EnsureComponent<PlayerInteractor>(player);

            Transform cameraTransform = ConfigureCamera(player.transform);
            motor.SetViewTransform(cameraTransform);
            interactor.SetRayOrigin(cameraTransform);

            EnsureComponent<BasicTreasureInteractable>(treasure);
            EnsureCollider(treasure);
            ConfigureWaterVolume();

            EditorUtility.SetDirty(inputReader);
            EditorUtility.SetDirty(waterSensor);
            EditorUtility.SetDirty(motor);
            EditorUtility.SetDirty(cursorLock);
            EditorUtility.SetDirty(interactor);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            EditorUtility.DisplayDialog(
                "TreasureDivers Milestone 1",
                "Prototype_DiveRoom is configured for first-person walking, jumping, swimming, cursor locking and basic interaction.",
                "Done");
        }

        [MenuItem("Tools/Treasure Divers/Milestone 1/Validate Prototype Dive Room Setup")]
        public static void ValidatePrototypeDiveRoomSetup()
        {
            if (!OpenPrototypeSceneIfNeeded())
            {
                return;
            }

            bool valid = true;
            valid &= ReportMissing<GameObject>(GameObject.Find("Player"), "Player object");
            valid &= ReportMissing<GameObject>(GameObject.Find("TestTreasure"), "TestTreasure object");

            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                valid &= ReportMissing(player.GetComponent<CharacterController>(), "CharacterController on Player");
                valid &= ReportMissing(player.GetComponent<DiverInputReader>(), "DiverInputReader on Player");
                valid &= ReportMissing(player.GetComponent<WaterSensor>(), "WaterSensor on Player");
                valid &= ReportMissing(player.GetComponent<FirstPersonDiverMotor>(), "FirstPersonDiverMotor on Player");
                valid &= ReportMissing(player.GetComponent<DiverCursorLock>(), "DiverCursorLock on Player");
                valid &= ReportMissing(player.GetComponent<PlayerInteractor>(), "PlayerInteractor on Player");
            }

            valid &= ReportMissing(Object.FindAnyObjectByType<WaterVolume>(), "WaterVolume in scene");

            if (valid)
            {
                Debug.Log("TreasureDivers Milestone 1 setup validation passed.");
            }
        }

        private static bool OpenPrototypeSceneIfNeeded()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (activeScene.path == PrototypeScenePath)
            {
                return true;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return false;
            }

            EditorSceneManager.OpenScene(PrototypeScenePath, OpenSceneMode.Single);
            return true;
        }

        private static GameObject FindRequiredObject(string objectName)
        {
            GameObject result = GameObject.Find(objectName);

            if (result == null)
            {
                throw new MissingReferenceException($"Could not find required scene object named '{objectName}'.");
            }

            return result;
        }

        private static T EnsureComponent<T>(GameObject target) where T : Component
        {
            T component = target.GetComponent<T>();

            if (component != null)
            {
                return component;
            }

            return Undo.AddComponent<T>(target);
        }

        private static Transform ConfigureCamera(Transform player)
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                Undo.RegisterCreatedObjectUndo(cameraObject, "Create Main Camera");
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            Undo.SetTransformParent(camera.transform, player, "Parent Main Camera To Player");
            Undo.RecordObject(camera.transform, "Configure First Person Camera");
            camera.transform.localPosition = new Vector3(0f, 1.62f, 0f);
            camera.transform.localRotation = Quaternion.identity;

            Undo.RecordObject(camera, "Configure Main Camera");
            camera.fieldOfView = 72f;
            camera.nearClipPlane = 0.03f;

            return camera.transform;
        }

        private static void ConfigureWaterVolume()
        {
            GameObject water = GameObject.Find("WaterVolume_Prototype");

            if (water == null)
            {
                water = new GameObject("WaterVolume_Prototype");
                Undo.RegisterCreatedObjectUndo(water, "Create Water Volume");
            }

            Undo.RecordObject(water.transform, "Configure Water Volume");
            water.transform.position = new Vector3(0f, -5f, 8f);
            water.transform.rotation = Quaternion.identity;
            water.transform.localScale = new Vector3(25f, 11f, 25f);

            BoxCollider boxCollider = EnsureComponent<BoxCollider>(water);
            Undo.RecordObject(boxCollider, "Configure Water Trigger");
            boxCollider.isTrigger = true;
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.one;

            EnsureComponent<WaterVolume>(water);
        }

        private static void EnsureCollider(GameObject target)
        {
            if (target.GetComponent<Collider>() != null)
            {
                return;
            }

            BoxCollider collider = Undo.AddComponent<BoxCollider>(target);
            collider.size = Vector3.one;
        }

        private static bool ReportMissing<T>(T value, string label) where T : Object
        {
            if (value != null)
            {
                return true;
            }

            Debug.LogError($"TreasureDivers Milestone 1 setup validation failed: missing {label}.");
            return false;
        }
    }
}
