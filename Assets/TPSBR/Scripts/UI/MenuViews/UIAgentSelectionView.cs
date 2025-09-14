using System;
using System.Linq;
using TMPro;
using UnityEngine;
using Cinemachine;

namespace TPSBR.UI
{
	public class UIAgentSelectionView : UICloseView
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private CinemachineVirtualCamera _camera;
		[SerializeField]
		private UIList _agentList;
		[SerializeField]
		private UIButton _selectButton;
		[SerializeField]
		private TextMeshProUGUI _agentName;
		[SerializeField]
		private TextMeshProUGUI _agentDescription;
		[SerializeField]
		private string _agentNameFormat = "{0}";
		[SerializeField]
		private UIBehaviour _selectedAgentGroup;
		[SerializeField]
		private UIBehaviour _selectedEffect;
		[SerializeField]
		private AudioSetup _selectedSound;
		[SerializeField]
		private float _closeDelayAfterSelection = 0.5f;

		private string _previewAgent;
		private AgentSetup[] _availableAgents;

		// UIView INTERFACE

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_agentList.SelectionChanged += OnSelectionChanged;
			_agentList.UpdateContent += OnListUpdateContent;

			_selectButton.onClick.AddListener(OnSelectButton);
		}

		private void OnListUpdateContent(int index, MonoBehaviour content)
		{
			var behaviour = content as UIBehaviour;
			
			if (_availableAgents != null && index >= 0 && index < _availableAgents.Length)
			{
				var setup = _availableAgents[index];
				behaviour.Image.sprite = setup.Icon;
			}
		}

		protected override void OnOpen()
		{
			base.OnOpen();

			_camera.enabled = true;
			_selectedEffect.SetActive(false);

			_previewAgent = Context.PlayerData.AgentID;

			// Get only available (owned + free) agents
			_availableAgents = GetAvailableAgents();
			
			_agentList.Refresh(_availableAgents.Length, false);
			
			UpdateAgent();
		}

		protected override void OnClose()
		{
			_camera.enabled = false;

			Context.PlayerPreview.ShowAgent(Context.PlayerData.AgentID);

			base.OnClose();
		}

		protected override void OnDeinitialize()
		{
			_agentList.SelectionChanged -= OnSelectionChanged;
			_agentList.UpdateContent -= OnListUpdateContent;

			_selectButton.onClick.RemoveListener(OnSelectButton);

			base.OnDeinitialize();
		}

		// PRIVATE METHODS

		private AgentSetup[] GetAvailableAgents()
		{
			var allAgents = Context.Settings.Agent.Agents;
			
			// Filter agents based on ownership
			var availableAgents = allAgents.Where(agent => 
			{
				// Always include free agents
				if (IsFreeAgent(agent.ID))
					return true;
					
				// Check if player owns this agent
				if (PlayerInventory.Instance != null)
				{
					return PlayerInventory.Instance.HasItem(agent.ID);
				}
				
				// If no PlayerInventory, only show free agents
				return false;
			}).ToArray();
			
			Debug.Log($"🎭 Available agents for selection: {availableAgents.Length}/{allAgents.Length}");
			foreach (var agent in availableAgents)
			{
				Debug.Log($"   - {agent.DisplayName} ({agent.ID})");
			}
			
			return availableAgents;
		}
		
		private bool IsFreeAgent(string agentID)
		{
			// List of agents that are free/default and don't need to be purchased
			return agentID == "Agent01" || agentID == "Agent.Marine";
		}

		private void OnSelectionChanged(int index)
		{
			if (_availableAgents != null && index >= 0 && index < _availableAgents.Length)
			{
				_previewAgent = _availableAgents[index].ID;
				UpdateAgent();
			}
		}

		private void OnSelectButton()
		{
			bool isSame = Context.PlayerData.AgentID == _previewAgent;

			if (isSame == false)
			{
				Context.PlayerData.AgentID = _previewAgent;

				_selectedEffect.SetActive(false);
				_selectedEffect.SetActive(true);

				PlaySound(_selectedSound);

				UpdateAgent();
				Invoke("CloseWithBack", _closeDelayAfterSelection);
			}
			else
			{
				CloseWithBack();
			}
		}

		private void UpdateAgent()
		{
			if (_availableAgents == null || _availableAgents.Length == 0)
			{
				// No available agents, hide everything
				Context.PlayerPreview.HideAgent();
				_agentName.text = "No Characters Available";
				_agentDescription.text = "Purchase characters from the shop to unlock them.";
				_selectedAgentGroup.SetActive(false);
				return;
			}
			
			_agentList.Selection = Array.FindIndex(_availableAgents, t => t.ID == _previewAgent);

			if (_agentList.Selection < 0)
			{
				_agentList.Selection = 0;
				_previewAgent = _availableAgents[_agentList.Selection].ID;
			}

			if (_previewAgent.HasValue() == false)
			{
				Context.PlayerPreview.HideAgent();
				_agentName.text = string.Empty;
				_agentDescription.text = string.Empty;
			}
			else
			{
				var setup = Context.Settings.Agent.GetAgentSetup(_previewAgent);

				Context.PlayerPreview.ShowAgent(_previewAgent);
				_agentName.text = string.Format(_agentNameFormat, setup.DisplayName);
				_agentDescription.text = setup.Description;
			}

			_selectedAgentGroup.SetActive(_previewAgent == Context.PlayerData.AgentID);
		}
	}
}
