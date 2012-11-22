def add(a,b):
    return a + b

class SimpleClass:
    def hello_simple(self,target):
	    return "Hello,%s!" % target

class MyInheritedType(InheritMe):
    def DoMath(self,input):
        return input ** 2

def add2(*args):
    return sum(args) 		
    
def ToString():
    return "From the plugin!"
    
Guid = "852bb7e1-1839-4a19-a9f0-78c1f6e29053"