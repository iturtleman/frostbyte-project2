import sys
from xml.dom.minidom import parse, parseString

if len(sys.argv) < 2:
  print "Please provide a filename"
  sys.exit()

dom = parse(sys.argv[1])

for elem in dom.getElementsByTagName("Animation"):
  reversed = elem.cloneNode(True).childNodes
  reversed.reverse()
  
  for c, r in zip(elem.childNodes, reversed):
    elem.replaceChild(r, c)
  
f = open("new.anim", 'w')
f.write(dom.toxml())
f.close()