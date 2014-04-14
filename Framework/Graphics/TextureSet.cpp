#include "stdafx.h"
#include <set>
#include <boost/algorithm/string.hpp>
#include <boost/lexical_cast.hpp>
#include "Shared/Algorithm.h"
#include "Shared/Foreach.h"
#include "Shared/StringTools.h"
#include "Shared/PathTools.h"
#include "Graphics/Texture.h"
#include "Engine/FileHandler.h"
#include "Engine/Globals.h"
#include "TextureSet.h"
#include "TextureSetReference.h"

namespace imf
{
	TextureSet::TextureSet(ImageMapPairsType& in_pairs)
	{
		std::pair<std::string, std::string> imageMapPair;
		imf_foreach (imageMapPair, in_pairs)
		{
			std::string& image(imageMapPair.first);
			std::string& map(imageMapPair.second);

			// Load texture
			boost::shared_ptr<Texture> pTexture(new Texture(image));

			// Load & parse map
			float divisor = Globals::GetHiResContentScale();
			std::vector<std::string> lines;
			FileHandler::ExtractFile(map, lines);
			CheckMsg(lines.size() > 0, "Map file non-existant or empty: " << map);

			imf_foreach (std::string& line, lines)
			{
				if (!line.empty())
				{
					std::vector<std::string> tokens;
					StringTools::Tokenize(line, "=", tokens);

					CheckMsg(tokens.size() == 2, "Invalid token count in map " << map << ", line: " << line);
					std::string idString = boost::algorithm::trim_copy(tokens[0]);
					std::string rectString = boost::algorithm::trim_copy(tokens[1]);

					std::vector<std::string> rectParts;
					StringTools::Tokenize(rectString, " ", rectParts);

					CheckMsg(rectParts.size() == 4, "Invalid rectangle token count in map " << map << ", line: " << line);

					std::string file;
					fpRect rect;

					try
					{
						file = idString;
						rect.x = ((float)boost::lexical_cast<int>(rectParts[0])) / divisor;
						rect.y = ((float)boost::lexical_cast<int>(rectParts[1])) / divisor;
						rect.w = ((float)boost::lexical_cast<int>(rectParts[2])) / divisor;
						rect.h = ((float)boost::lexical_cast<int>(rectParts[3])) / divisor;
					}
					catch (...)
					{
						CheckMsg(false, "Invalid line in map file: " << map);
					}

					CheckMsg(!imf::contains_key(textureDataSet, file), file);
					TextureData data(pTexture, rect);
					textureDataSet[file] = data;
				}
			}
		}
	}

	TextureSet::TextureSet(std::vector<std::string>& in_images)
	{
		float divisor = Globals::GetCurrentContentScale();

		// Load textures
		imf_foreach (std::string& path, in_images)
		{
			boost::shared_ptr<Texture> pTexture(new Texture(path));

			std::string fileId = PathTools::GetFilePathWithoutExtension(PathTools::GetFile(path));
			iVector2 size = pTexture->GetSize();
			iRect rect(0, 0, (float)size.x / divisor, (float)size.y / divisor);

			Check(!imf::contains_key(textureDataSet, fileId));
			TextureData data(pTexture, rect);
			textureDataSet[fileId] = data;
		}
	}

	TextureSet* TextureSet::CreateFromListFile(const std::string& in_atlasListFile)
	{
		const std::string spriteSetFolder = PathTools::GetPath(in_atlasListFile);

		std::vector<std::string> filelist;
		if (!FileHandler::ExtractFile(in_atlasListFile, filelist))
		{
			CheckMsg(false, "Texture set list file missing: " << in_atlasListFile);
		}
		
		if (!filelist.empty() && filelist[0] == "atlases:")
		{
			// Atlases
			TextureSet::ImageMapPairsType atlasPairs;

			for (uint i = 1; i < filelist.size(); ++i)
			{
				const std::string atlasPath = PathTools::Join(spriteSetFolder, filelist[i]);
				const std::string texPath = PathTools::GetFilePathWithoutExtension(atlasPath) + ".png";
				
				atlasPairs.push_back(std::make_pair(texPath, atlasPath));
			}
			
			IMF_LOG(SPRITE, INFO, "Found " << atlasPairs.size() << " atlas textures for texture set \"" << in_atlasListFile << "\".");
			
			// Load atlas
			return new TextureSet(atlasPairs);
		}
		else
		{
			// Separate textures
			imf_foreach (std::string& texPath, filelist)
			{
				texPath = PathTools::Join(spriteSetFolder, texPath);
			}
			
			IMF_LOG(SPRITE, INFO, "Found " << filelist.size() << " textures for texture set \"" << in_atlasListFile << "\".");
			
			return new TextureSet(filelist);
		}
	}

	TextureSet::~TextureSet()
	{
		textureDataSet.clear();
		// Flush textures?
	}

	bool TextureSet::GetTexture(const std::string& in_name, TextureSetReference& out_texture)
	{
		fpRect r;
		TexturesType::iterator iter = textureDataSet.find(in_name);
		if (iter != textureDataSet.end())
		{
			out_texture = TextureSetReference(in_name, name, iter->second.pTexture.get(), iter->second.area);
			return true;
		}

		return false;
	}
}
