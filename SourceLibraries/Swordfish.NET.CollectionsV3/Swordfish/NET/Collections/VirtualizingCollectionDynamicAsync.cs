using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Swordfish.NET.Collections;

public class VirtualizingCollectionDynamicAsync<T> : VirtualizingCollectionAsync<T> where T : class
{
	private System.Timers.Timer timer;

	public event EventHandler CountChanged;

	public VirtualizingCollectionDynamicAsync(IVirtualizingCollectionItemsProvider<T> itemsProvider, int pageSize, int pageTimeout, int loadCountInterval)
		: base(itemsProvider, pageSize, pageTimeout)
	{
		timer = new System.Timers.Timer(loadCountInterval);
		timer.AutoReset = false;
		timer.Elapsed += TimerCallback;
		itemsProvider.CountChanged += ItemsProvider_CountChanged;
		ItemsProvider_CountChanged(null, null);
	}

	private void ItemsProvider_CountChanged(object sender, EventArgs e)
	{
		if (!timer.Enabled)
		{
			timer.Enabled = true;
			Task.Run(delegate
			{
				Thread.Sleep(20);
				LoadCount();
			});
		}
	}

	protected override void LoadCountCompleted(object args)
	{
		base.LoadCountCompleted(args);
		this.CountChanged?.Invoke(this, EventArgs.Empty);
	}

	private void TimerCallback(object sender, EventArgs args)
	{
		LoadCount();
	}
}
