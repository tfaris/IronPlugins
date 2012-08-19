IronPlugins
===========

A library intended to make creating .NET plugins/extensions using iron languages more simple.

IronPlugins can be created from a string:
```
dynamic plugin = StringPlugin(
@"def add(a,b):
    return a+b");
Console.WriteLine)plugin.add(10,55));

//Output
//65
```

or from a file:

```
//my_plugin.py
def add(*args):
    return sum(args)
    
// C#
dynamic plugin = FilePlugin("my_plugin.py");
Console.WriteLine(plugin.add(1,2,3,4,5));

//Output
//15
```

By using a PluginManager, we can monitor file-based plugins for changes, and reload them automatically.

```
PluginManager mgr = new PluginManager();
mgr.PluginReloaded += _pluginManager_PluginReloaded;
mgr.AddPlugin(FilePlugin("my_plugin.py"));
// Any changes made to the my_plugin.py file will be auto-reloaded, any new
// attributes added will be accessible instantly.

void _pluginManager_PluginReloaded(object sender, PluginReloadEventArgs args)
{
  // This event will tell us which plugin was reloaded.
}
```

PluginManager also has a global context, which can be accessed through plugins using the ```__mgr__``` attribute.
This attribute can be accessed in an indexed-dictionary fashion. We can get and set.
```
// my_plugin.py
app_data = __mgr__["sample_data"]
for sample in app_data:
    # Do some reporting, statistics, logging, or anything else!
__mgr__["results"] = 123
```

