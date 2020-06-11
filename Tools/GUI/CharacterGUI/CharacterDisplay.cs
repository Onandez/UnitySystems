using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameKit.Tools
{
    public class CharacterDisplay : CharacterCanvas
    {
        public Text characterName;
        public Button moveButton;
        public Button attackButton;
        public Button endTrunButton;

        protected override void Start()
        {
            base.Start();

        }

        protected override void Update()
        {
            
        }

        // Updates the bar
        public override void UpdateHealthBar(float currentHealth, float minHealth, float maxHealth, bool show)
        {
            base.UpdateHealthBar(currentHealth, minHealth, maxHealth, show);
        }

        // Updates the bar
        public override void UpdateStaminaBar(float currentStamina, float minStamina, float maxStamina, bool show)
        {
            base.UpdateStaminaBar(currentStamina, minStamina, maxStamina, show);
        }

        /*
         * Revisar TBS Engine chararter display, character portrqit charter hover
         * public override void SetupCharacter(Character character)
        {
            base.SetupCharacter(character);

            characterName.text = character.characterID;
        }*/

        public void SelectAction(string selectedAction)
        {
            //m_Character.SelectAction(selectedAction);

        }
    }
}
