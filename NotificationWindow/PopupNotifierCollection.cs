using System.Collections.Generic;
using System.Collections;

namespace NotificationWindow
{
    public class PopupNotifierCollection : System.Collections.CollectionBase
	{
		public PopupNotifierCollection(int pi_maxPopUp)
		{
			mi_maxPopUp = pi_maxPopUp;
		}

		private int mi_maxPopUp;
        private int displaycount = 0 ;
		private Dictionary<int, ArrayList> md_PopUpList = new Dictionary<int, ArrayList>();

		public PopupNotifier this[int index]
		{
			get { return (PopupNotifier)List[index]; }
			set { List[index] = value; }
		}

		public int Add(PopupNotifier item)
		{
		  //  item.Close += new System.EventHandler(item_Close);
			item.HideNotif += new System.EventHandler(item_Hide);

			int layer = 0;
			int index = 0;
			findNextPopUp(out layer, out index);
			item.iPopUpLayer = layer;
			item.iPopUpIndex = index;
			return this.List.Add(item);
		}



		void item_Hide(object sender, System.EventArgs e)
		{
			Remove((PopupNotifier)sender);
			FixPositions((PopupNotifier)sender);
            displaycount--;

            if ((displaycount == 0) && (List.Count > 0))
            {
                Popup();
            }
			
		}


		public void FixPositions(PopupNotifier pop)
		{
			foreach (PopupNotifier popN in List)
			{
				PopupNotifierForm window = popN.form;
				
				if (window != pop.form)
				{
					if (window.Top < pop.form.Top && window.Left == pop.form.Left)
					{
						window.Top = window.Top + pop.form.Height;
					}
				}
			}

		}

		
		void item_Close(object sender, System.EventArgs e)
		{
			//Console.WriteLine("close");
		//	Remove((PopupNotifier)sender);
		}


		public bool Contains(PopupNotifier item)
		{
			return this.List.Contains(item);
		}

		public void Remove(PopupNotifier item)
		{
			removePopUp(item.iPopUpLayer, item.iPopUpIndex);
			try
			{
				this.List.Remove(item);
			}
			catch
			{ }
		}

		public int IndexOf(PopupNotifier item)
		{
			return this.List.IndexOf(item);
		}

		public void Popup()
		{
            displaycount = 0;
            foreach (PopupNotifier item in List)
            {
                item.Popup();
                displaycount++;
                if (displaycount == mi_maxPopUp) { break; }
            }



        }

		private void removePopUp(int layer, int index)
		{
			md_PopUpList[layer].Remove(index);
			if (md_PopUpList[layer].Count == 0)
			{
				md_PopUpList.Remove(layer);
			}
		}

		private void findNextPopUp(out int layer, out int index)
		{
			layer = 0;
			index = 0;
			int numLayers = md_PopUpList.Count;

			for (int i = 0; i < numLayers; i++)
			{
				if (md_PopUpList[i].Count >= mi_maxPopUp)
				{
					layer++;
					continue;
				}
				else
				{
					layer = i;
					md_PopUpList[i].Sort();
					int tmpIndex = 0;
					foreach (int j in md_PopUpList[i])
					{
						if (j != tmpIndex)
						{
							break;
						}
						tmpIndex++;
					}
					index = tmpIndex;
					break;
				}
			}

			addNextPopUp(layer, index);
		}

		private void addNextPopUp(int layer, int index)
		{
			if (md_PopUpList.ContainsKey(layer))
			{
				md_PopUpList[layer].Add(index);
			}
			else
			{
				ArrayList li_counter = new ArrayList();
				li_counter.Add(index);
				md_PopUpList.Add(layer, li_counter);
			}
		}
	}
}
