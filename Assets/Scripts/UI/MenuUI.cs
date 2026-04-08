using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public Button MissionBtn;
    public Button backStoryBtn;
    public Button InstructionBtn;
    public TextMeshProUGUI backStoryText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI MissionText;



    //on button click if back story btn it should disable instruction text and enable back story text and vice versa for instruction btn
    public void onBackPress()
    {
        backStoryText.gameObject.SetActive(true);
        instructionText.gameObject.SetActive(false);
        MissionText.gameObject.SetActive(false);
    }
    public void onInstructionPress()
    {

        backStoryText.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(true);
        MissionText.gameObject.SetActive(false);
    }
    public void onMissionPress()
    {
        backStoryText.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(false);
        MissionText.gameObject.SetActive(true);
    }







}
