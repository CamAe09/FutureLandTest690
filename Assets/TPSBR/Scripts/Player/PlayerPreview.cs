using UnityEngine;
using Plugins.Outline;

namespace TPSBR
{
	public class PlayerPreview : CoreBehaviour
	{
		// PUBLIC MEMBERS

		public string AgentID => _agentID;

		// PRIVATE MEMBERS

		[SerializeField]
		private Transform _agentParent;

		private string _agentID;
		private GameObject _agentInstance;

		private OutlineBehaviour _outline;

		// PUBLIC METHODS

		public void ShowAgent(string agentID, bool force = false)
		{
			if (agentID == _agentID && force == false)
				return;

			// Check if player owns this agent (unless it's a default free agent)
			if (agentID.HasValue() && !IsFreeAgent(agentID))
			{
				if (PlayerInventory.Instance != null)
				{
					bool ownsAgent = PlayerInventory.Instance.HasItem(agentID);
					if (!ownsAgent)
					{
						Debug.Log($"🔒 Player does not own agent {agentID}, cannot preview it. Purchase it from the shop first!");
						return;
					}
				}
			}

			ClearAgent();
			InstantiateAgent(agentID);
		}

		public void ShowOutline(bool value)
		{
			_outline.enabled = value;
		}

		public void HideAgent()
		{
			ClearAgent();
		}

		// MONOBEHAVIOUR

		protected void Awake()
		{
			_outline = GetComponentInChildren<OutlineBehaviour>(true);
			_outline.enabled = false;
		}

		// PRIVATE METHODS

		private bool IsFreeAgent(string agentID)
		{
			// List of agents that are free/default and don't need to be purchased
			// Agent01 = Default agent, Agent.Marine = Free marine character  
			return agentID == "Agent01" || agentID == "Agent.Marine";
		}

		private void InstantiateAgent(string agentID)
		{
			if (agentID.HasValue() == false)
				return;

			var agentSetup = Global.Settings.Agent.GetAgentSetup(agentID);

			if (agentSetup == null)
				return;

			_agentInstance = Instantiate(agentSetup.MenuAgentPrefab, _agentParent);
			_agentID = agentID;
		}

		private void ClearAgent()
		{
			_agentID = null;

			if (_agentInstance == null)
				return;

			_outline.enabled = false;

			Destroy(_agentInstance);
			_agentInstance = null;
		}
	}
}
