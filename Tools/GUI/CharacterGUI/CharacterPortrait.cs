using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameKit.Tools
{
    public class CharacterPortrait : CharacterCanvas
    {
        public Button characerButton;

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

        public void SelectCharacter()
        {
            //m_Character.CanBeDeselected();

        }
    }
}
