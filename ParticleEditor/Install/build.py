import sys, os, getopt, shutil, struct, platform
import tempfile
import xml.etree.ElementTree as ET

configuration = 'Release'
this_dir = os.path.dirname(os.path.abspath(__file__))
proj_path = os.path.join(this_dir, '..')
bin_path = os.path.join(proj_path, 'bin', configuration)

#xbuild /property:Configuration=Release ParticleEditor.csproj

build = 

mono = "mono "
binpath = os.path.abspath(os.path.join(base_path, 'Build/Bin'))
vex = os.path.join(binpath, 'VexToSprite.exe')
sspack = os.path.join(binpath, 'sspack.exe')

if platform.system() == 'Windows':
	mono = ''

from mg.tools import *
from mg.manifest import *

###############################################################################
class Usage(Exception):
	def __init__(self, msg):
		self.msg = msg
		
###############################################################################
def write_temporary_file(content):
	(tmpfilefd, tmpname) = tempfile.mkstemp(suffix='.txt', text=True)	
	tmpfile = os.fdopen(tmpfilefd, 'w')
	tmpfile.write(content)
	tmpfile.close()
	return tmpname

###############################################################################
def build_spriteset_binary(inputFile, outputFile):
	# open files
	tree = ET.parse(inputFile)
	root = tree.getroot()
	out = open(outputFile, 'wb')
	
	spriteDefinitions = root.findall('SpriteDefinition')
	
	def encode7bit(value):
		temp = value
		bytes = ''
		while temp >= 128:
			bytes += chr(0x000000FF & (temp | 0x80))			
			temp >>= 7
		bytes += chr(temp)
		return bytes
	
	def outputString(string, out):
		string = string.encode('utf-8')
		lenBytes = encode7bit(len(string))
		
		for b in lenBytes:
			out.write(struct.pack('<B', ord(b)))
		out.write(struct.pack('<' + str(len(string)) + 's', string))
	
	def outputTransform(transform, out):		
		out.write(struct.pack('<II ff ff f ffff B', 
			int(transform.find('StartFrame').text),
			int(transform.find('EndFrame').text),
			float(transform.find('Position/X').text),
			float(transform.find('Position/Y').text),
			float(transform.find('Scale/X').text),
			float(transform.find('Scale/Y').text),
			float(transform.find('Rotation').text),
			float(transform.find('RedMul').text),
			float(transform.find('GreenMul').text),
			float(transform.find('BlueMul').text),
			float(transform.find('AlphaMul').text),
			(transform.find('Visible').text == 'true')
			))
	
	# header
	out.write(b'mgss')
	
	# main spriteset attributes
	out.write(struct.pack('<IIII', 
		int(root.attrib['Width']), 
		int(root.attrib['Height']), 
		int(root.attrib['FrameRate']),
		int(len(spriteDefinitions))))
	
	# sprite defs
	for spriteDefinition in spriteDefinitions:
		spriteInstances = spriteDefinition.findall('Instances/SpriteInstance')
		labels = spriteDefinition.findall('Labels/SerializableKeyValuePairOfUInt32String')
		shapes = spriteDefinition.findall('Shapes/SpriteShape')
	
		outputString(spriteDefinition.get('Name'), out)
		outputString(spriteDefinition.get('LinkageName'), out)		
		
		out.write(struct.pack('<I ff III',
			int(spriteDefinition.get('Id')),			
			float(spriteDefinition.get('OffsetX')),
			float(spriteDefinition.get('OffsetY')),
			len(spriteInstances),
			len(labels),
			len(shapes)
			))
		
		for spriteInstance in spriteInstances:
			transforms = spriteInstance.findall('Transforms')
			
			outputString(spriteInstance.get('InstanceName'), out)
			outputString(spriteInstance.get('DefinitionName'), out)
			
			out.write(struct.pack('<f ff f ff II ffff B I',				
				float(spriteInstance.get('Depth')),
				float(spriteInstance.get('X')),
				float(spriteInstance.get('Y')),
				float(spriteInstance.get('Rotation')),
				float(spriteInstance.get('ScaleX')),
				float(spriteInstance.get('ScaleY')),
				int(spriteInstance.get('StartFrame')),
				int(spriteInstance.get('EndFrame')),
				float(spriteInstance.get('RedMul')),
				float(spriteInstance.get('GreenMul')),
				float(spriteInstance.get('BlueMul')),
				float(spriteInstance.get('AlphaMul')),
				(spriteInstance.get('Visible')) == "true",
				len(transforms)
				))
				
			for transform in transforms:
				outputTransform(transform, out)
		
		for label in labels:
			out.write(struct.pack('<I', int(label.find('Key').text)))
			outputString(label.find('Value').text, out)
			
		for shape in shapes:					
			shapeType = { 
							'Circle' : 0,
							'Rectangle' : 1,
							'Point' : 2,
							'Polygon' : 3
						} [shape.get('ShapeType')]
			
			transforms = shape.findall('Transforms/SpriteGenericTransform')
			shapeData = shape.findall('Data/float')
			
			outputString(shape.find('Name').text, out)
			out.write(struct.pack('<B II',
				shapeType,
				len(transforms),
				len(shapeData)
				))
			
			for transform in transforms:
				outputTransform(transform, out)
				
			for data in shapeData:
				out.write(struct.pack('<f', float(data.text)))
	
	out.close()
	
###############################################################################
def build_sprites(dir):
	textureSize = 2048

	# compile the sprites
	files = scan_files(os.path.join(input_base_path, dir), True, ['swf'])
	
	for file in files:
		atlasName = os.path.splitext(os.path.basename(file))[0]
		manifest = os.path.join(manifests_path, os.path.basename(file)) + '.man'
				
		if modified(manifest, file):		
			# clear out old output
			targetPath = os.path.join(output_base_path, dir, atlasName)
			vexOutput = os.path.join(os.path.dirname(file), atlasName)
			resourcesPath = os.path.join(os.path.dirname(file), 'Resources_' + atlasName)
			
			shutil.rmtree(targetPath, True)
			shutil.rmtree(vexOutput, True)
			shutil.rmtree(resourcesPath, True)
			
			# compile swf
			commandline = mono + '"' + vex + '" "' + file + '"'
			run_cmd(commandline, printOutput = True)
		
			# prepare atlas packing			
			make_dirs(targetPath)
							
			# scan for input files, put them in a temp file (command line gets too long for windows to handle)
			atlasFilesList = scan_files(vexOutput, True, ['png'])
			atlasFilesStr = ""
			for atlasFile in atlasFilesList:
				atlasFilesStr += "/clamp:" + atlasFile + '\n'
			atlasFiles = write_temporary_file(atlasFilesStr)
			
			# call the atlas packer
			commandline = (mono + '"' + sspack + '"'
						' /mw:' + str(textureSize) + 
						' /mh:' + str(textureSize) + 
						' /name:"' + atlasName + '"'
						' /path:"' + targetPath + '"'
						' /il:"' + atlasFiles + '"'
						)		
			run_cmd(commandline, printOutput = True)
					
			# compile the xml			
			definitionNameXml = atlasName + '.xml'
			definitionNameBin = atlasName + '.bin'
			build_spriteset_binary(os.path.join(vexOutput, definitionNameXml), os.path.join(vexOutput, definitionNameBin))
			
			# move the binary to the output folder
			shutil.copy(os.path.join(vexOutput, definitionNameBin), os.path.join(targetPath, definitionNameBin))
			
			# write the modification manifest
			write_manifest(manifest, [file])
		
###############################################################################
def build_tiles(dir):
	textureSize = 2048

	# prepare atlas
	atlasName = dir	
	targetPath = os.path.join(output_base_path, dir)
	make_dirs(targetPath)
	
	# find all textures
	atlasFilesList = scan_files(os.path.join(input_base_path, dir), True, ['png'])
	atlasFilesStr = ""
	for atlasFile in atlasFilesList:
		atlasFilesStr += "/clamp:" + atlasFile + '\n'
	atlasFiles = write_temporary_file(atlasFilesStr)
				
	# call the atlas packer
	commandline = (mono + '"' + sspack + '"'
				' /mw:' + str(textureSize) + 
				' /mh:' + str(textureSize) + 
				' /name:"' + atlasName + '"'
				' /path:"' + targetPath + '"'
				' /il:"' + atlasFiles + '"'
				)
	
	run_cmd(commandline, printOutput = True)

###############################################################################
def copy_content(dir, extensions):
	fullSourceDir = os.path.join(input_base_path, dir)
	files = scan_files(fullSourceDir, True, extensions)
	
	for file in files:
		fileDirName = os.path.dirname(file)
		fileBaseName = os.path.basename(file)
		fileAndSubPath = fileDirName[len(input_base_path) + 1:]				
		path = os.path.join(output_base_path, fileAndSubPath)
		make_dirs(path)
		shutil.copy(file, os.path.join(path, fileBaseName))

###############################################################################
def build():
	# ensure directories
	make_dirs(output_base_path)
	make_dirs(manifests_path)	
	
	# ensure tools
	if not os.path.exists(vex):
		print_error('The vex tool is missing. Expected location: ' + vex)
	
	if not os.path.exists(sspack):
		print_error('The sspack tool is missing. Expected location: ' + sspack)
	
	# copy dlls
	shutil.copy(os.path.join(externals_path, "OpenAL/OpenAL32.dll"), bundle_path)
	#shutil.copy(os.path.join(externals_path, "OpenAL/wrap_oal.dll"), bundle_path)
	
	# build sprite maps
	build_sprites('Sprites')		
	build_sprites('Menus')
			
	# copy audio files
	copy_content('Audio', ['ogg', 'wav', 'xml'])
	
	# copy fonts
	copy_content('Fonts', ['fnt', 'png'])
	
	# copy settings
	copy_content('Settings', ['xml'])
	
	# copy textures
	copy_content('Textures', ['png'])
	
	# copy tile data
	copy_content('Tiles', ['xml'])
	
	# build tile atlases
	build_tiles('Tiles')
	
	# copy input maps
	copy_content('Input', ['xml'])
	
	# copy definitions
	copy_content('Definitions', ['xml'])
	
	# copy translations
	copy_content('Language', ['csv'])

###############################################################################
def clean():
	print("todo")
	# todo

###############################################################################
def main(argv=None):
	doClean = False
	
	if argv is None:
		argv = sys.argv
	try:
		try:
			opts, args = getopt.getopt(argv[1:], "hqc", ["help", "quiet", "clean"])
		except getopt.error as msg:
			raise Usage(msg)

		# option processing
		for option, value in opts:
			if option in ("-h", "--help"):
				raise Usage(help_message)
			if option in ("-q", "--quiet"):
				config.silent = True
			if option in ("-c", "--clean"):
				doClean = True
		
		if not doClean:
			build()
		else:
			clean()
	
	except Usage as err:
		print(str(err.msg))
		return 2
	#except Exception as err:
	#	print(str(err))
	#	return 2
	
	print("Success.")
	return 0

###############################################################################
if __name__ == "__main__":
	sys.exit(main())