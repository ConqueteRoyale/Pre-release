// Selection d'unite Syteme de base de chaque RTS
// Par Nguyen Hoai Nguyen (12-11-2018), Modifie sur le code de base de Jeff Zimmer
//Kevin Langlois Sélection des unités à l'aide de UI
// https://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class SelectionUnit : MonoBehaviour
{
    [Header("Declaration pour le systeme de selection")]
    bool isSelecting = false;
    float newScale = 0.3f;
    Projector leProjecteur;
    Vector3 mousePosition1;

    [Header("Prefab cercle selected")]
    public GameObject selectionCirclePrefab;
    public GameObject hoverCirclePrefab;

    [Header("Bouton du UI")]
    public Button btnSelectionChevalier;
    public Button btnSelectionArcher;
    public Button btnSelectionLancier;
    public Button btnSelectionTous;

    [SerializeField]
    private KeyCode selectChevalier = KeyCode.Alpha7;
    private KeyCode selectArcher = KeyCode.Alpha5;
    private KeyCode selectLancier = KeyCode.Alpha6;
    private KeyCode selectTous = KeyCode.Alpha8;


    //Kevin Langlois Les boutons du UI permettent de faire la sélection des différents unités par types
    private void Start()
    {
        //Button btn5 = btnSelectionArcher.GetComponent<Button>();
        Button btn6 = btnSelectionLancier.GetComponent<Button>();
        Button btn7 = btnSelectionChevalier.GetComponent<Button>();
        Button btn8 = btnSelectionTous.GetComponent<Button>();

        //btn5.onClick.AddListener(delegate { GestionSelectionUnitBtn("archer"); });
        btn6.onClick.AddListener(delegate { GestionSelectionUnitBtn("lancier"); });
        btn7.onClick.AddListener(delegate { GestionSelectionUnitBtn("chevalier"); });
        btn8.onClick.AddListener(delegate { GestionSelectionUnitBtn("tous"); });
    }


    void Update()
    {

        GestionSelectionKey();

        // Cle visuel pour le joueur pour selection d'un unite
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
                {
                    if (selectableObject.hoverCircle == null)
                    {
                        //Instancier le cercle prefab sur le personnage
                        selectableObject.hoverCircle = Instantiate(hoverCirclePrefab);
                        selectableObject.hoverCircle.transform.SetParent(selectableObject.transform, false);
                        selectableObject.hoverCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                        leProjecteur = selectableObject.hoverCircle.GetComponent<Projector>();
                        leProjecteur.orthographicSize = newScale;
                    }
                }
            }
            else
            {
                foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
                {
                    Destroy(selectableObject.hoverCircle);
                    selectableObject.hoverCircle = null;
                }
            }
        }

        // Selection du personnage lorsqu'on click dessus
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray2, out hit, Mathf.Infinity))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
                {
                    var selectedObjects = new List<SelectableUnit>();

                    foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
                    {
                        if (selectableObject.selectionCircle == null && selectableObject.hoverCircle != null)
                        {
                            // Detruire le place holder
                            Destroy(selectableObject.hoverCircle);
                            selectableObject.hoverCircle = null;
                            //Instancier le cercle prefab sur le personnage
                            selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                            selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                            selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                            // Associé un tag Friendly au personnage sélectionné
                            selectableObject.transform.tag = "Friendly";


                            // Change la grosseur du cercle
                            leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                            leProjecteur.orthographicSize = newScale;
                        }
                    }
                }
            }
        }

        // If we press the left mouse button, begin selection and remember the location of the mouse
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            mousePosition1 = Input.mousePosition;

            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {
                if (selectableObject.selectionCircle != null)
                {
                    Destroy(selectableObject.selectionCircle.gameObject);
                    selectableObject.selectionCircle = null;
                }
            }
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0))
        {
            var selectedObjects = new List<SelectableUnit>();
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {
                if (IsWithinSelectionBounds(selectableObject.gameObject))
                {
                    selectedObjects.Add(selectableObject);
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Nombre d'unités :", selectedObjects.Count));
            foreach (var selectedObject in selectedObjects)
                sb.AppendLine("-> " + selectedObject.gameObject.name);

            isSelecting = false;
        }

        // Highlight all objects within the selection box
        if (isSelecting)
        {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {
                if (IsWithinSelectionBounds(selectableObject.gameObject))
                {
                    if (selectableObject.selectionCircle == null)
                    {
                        Destroy(selectableObject.hoverCircle);
                        selectableObject.hoverCircle = null;
                        selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                        selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                        // Associé un tag Friendly au personnage sélectionné
                        selectableObject.transform.tag = "Friendly";

                        // Change la grosseur du cercle
                        leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                        leProjecteur.orthographicSize = newScale;

                    }
                }
                else
                {
                    selectableObject.transform.tag = "Untagged";

                    if (selectableObject.selectionCircle != null)
                    {
                        Destroy(selectableObject.selectionCircle.gameObject);
                        selectableObject.selectionCircle = null;
                    }
                }
            }
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!isSelecting)
            return false;

        var camera = Camera.main;
        var viewportBounds = Utils.GetViewportBounds(camera, mousePosition1, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            // Create a rect from both mouse positions
            var rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
    //Kevin Langlois Permet la sélection des différents unités à l'aide des touches alphanumérique 6 7 8 9
    private void GestionSelectionKey()
    {
        if (Input.GetKeyDown(selectArcher))
        {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {
                if (selectableObject.unitClassName == "archer")
                {
                    if (selectableObject.selectionCircle == null)
                    {
                        selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                        selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                        // Associé un tag Friendly au personnage sélectionné
                        selectableObject.transform.tag = "Friendly";

                        // Change la grosseur du cercle
                        leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                        leProjecteur.orthographicSize = newScale;

                    }
                }
            }
        }

        if (Input.GetKeyDown(selectChevalier))
        {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {
                if (selectableObject.unitClassName == "chevalier")
                {
                    if (selectableObject.selectionCircle == null)
                    {
                        selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                        selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                        // Associé un tag Friendly au personnage sélectionné
                        selectableObject.transform.tag = "Friendly";

                        // Change la grosseur du cercle
                        leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                        leProjecteur.orthographicSize = newScale;

                    }
                }
            }
        }

        if (Input.GetKeyDown(selectLancier))
        {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {
                if (selectableObject.unitClassName == "lancier")
                {
                    if (selectableObject.selectionCircle == null)
                    {
                        selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                        selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                        // Associé un tag Friendly au personnage sélectionné
                        selectableObject.transform.tag = "Friendly";

                        // Change la grosseur du cercle
                        leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                        leProjecteur.orthographicSize = newScale;

                    }
                }
            }
        }

        if (Input.GetKeyDown(selectTous))
        {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {

                if (selectableObject.selectionCircle == null)
                {
                    selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                    selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                    selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                    // Associé un tag Friendly au personnage sélectionné
                    selectableObject.transform.tag = "Friendly";

                    // Change la grosseur du cercle
                    leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                    leProjecteur.orthographicSize = newScale;

                }


            }
        }

    }
    //Kevin Langlois Permet la sélection des différents unités à l'aide des boutons présents sur le UI
    private void GestionSelectionUnitBtn(string type)
    {
        //Chevalier
        if (type == "chevalier")
        {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {
                if (selectableObject.unitClassName == "chevalier")
                {
                    if (selectableObject.selectionCircle == null)
                    {
                        selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                        selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                        // Associé un tag Friendly au personnage sélectionné
                        selectableObject.transform.tag = "Friendly";

                        // Change la grosseur du cercle
                        leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                        leProjecteur.orthographicSize = newScale;

                    }
                }
            }
        }
        //Archer
        if (type == "archer")
        {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {
                if (selectableObject.unitClassName == "archer")
                {
                    if (selectableObject.selectionCircle == null)
                    {
                        selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                        selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                        // Associé un tag Friendly au personnage sélectionné
                        selectableObject.transform.tag = "Friendly";

                        // Change la grosseur du cercle
                        leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                        leProjecteur.orthographicSize = newScale;

                    }
                }
            }
        }
        //Lancier
        if (type == "lancier")
        {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {
                if (selectableObject.unitClassName == "lancier")
                {
                    if (selectableObject.selectionCircle == null)
                    {
                        selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                        selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                        // Associé un tag Friendly au personnage sélectionné
                        selectableObject.transform.tag = "Friendly";

                        // Change la grosseur du cercle
                        leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                        leProjecteur.orthographicSize = newScale;

                    }
                }
            }
        }
        //Tous
        if (type == "tous")
        {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
            {

                if (selectableObject.selectionCircle == null)
                {
                    selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                    selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                    selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);

                    // Associé un tag Friendly au personnage sélectionné
                    selectableObject.transform.tag = "Friendly";

                    // Change la grosseur du cercle
                    leProjecteur = selectableObject.selectionCircle.GetComponent<Projector>();
                    leProjecteur.orthographicSize = newScale;

                }


            }
        }
    }



}



