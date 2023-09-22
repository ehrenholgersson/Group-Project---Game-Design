using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectionBox : MonoBehaviour
{
    private Color _color;
    //[SerializeField] private List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();
    //[SerializeField] private List<Image> _images = new List<Image>();
    [SerializeField] private List<Image> _box1 = new List<Image>();
    [SerializeField] private List<Image> _box2 = new List<Image>();
    [SerializeField] private List<Image> _box3 = new List<Image>();
    [SerializeField] private List<Image> _box4 = new List<Image>();

    //public void SetColor(Color color)
    //{
    //    _color = color;
    //    //foreach (SpriteRenderer s in _spriteRenderers)
    //    //    s.color = _color;
    //    foreach (Image i in _images)
    //        i.color = _color;
    //}
    public void Set(List<int> players)
    {
        foreach (Image im in _box1)
            im.color = new Color(0,0,0,0);
        foreach (Image im in _box2)
            im.color = new Color(0, 0, 0, 0);
        foreach (Image im in _box3)
            im.color = new Color(0, 0, 0, 0);
        foreach (Image im in _box4)
            im.color = new Color(0, 0, 0, 0);

        int count = 1;
        foreach (int i in players)
        {
            switch (i) // red blue green yellow
            {
                case 1:

                        switch(count)
                        {
                            case 1:
                            foreach (Image im in _box1)
                                im.color = Color.red;
                            break;
                        case 2:
                            foreach (Image im in _box2)
                                im.color = Color.red;
                            break;
                        case 3:
                            foreach (Image im in _box3)
                                im.color = Color.red;
                            break;
                        case 4:
                            foreach (Image im in _box4)
                                im.color = Color.red;
                            break;
                        }
                            
                        
                    break;
                case 2:
                    switch (count)
                    {
                        case 1:
                            foreach (Image im in _box1)
                                im.color = Color.blue;
                            break;
                        case 2:
                            foreach (Image im in _box2)
                                im.color = Color.blue;
                            break;
                        case 3:
                            foreach (Image im in _box3)
                                im.color = Color.blue;
                            break;
                        case 4:
                            foreach (Image im in _box4)
                                im.color = Color.blue;
                            break;
                    }
                    break;
                case 3:
                    switch (count)
                    {
                        case 1:
                            foreach (Image im in _box1)
                                im.color = Color.green;
                            break;
                        case 2:
                            foreach (Image im in _box2)
                                im.color = Color.green;
                            break;
                        case 3:
                            foreach (Image im in _box3)
                                im.color = Color.green;
                            break;
                        case 4:
                            foreach (Image im in _box4)
                                im.color = Color.green;
                            break;
                    }
                    break;
                case 4:
                    switch (count)
                    {
                        case 1:
                            foreach (Image m in _box1)
                                m.color = Color.yellow;
                            break;
                        case 2:
                            foreach (Image m in _box2)
                                m.color = Color.yellow;
                            break;
                        case 3:
                            foreach (Image m in _box3)
                                m.color = Color.yellow;
                            break;
                        case 4:
                            foreach (Image m in _box4)
                                m.color = Color.yellow;
                            break;
                    }
                    break;
            }
            count++;
        }
    }

}
