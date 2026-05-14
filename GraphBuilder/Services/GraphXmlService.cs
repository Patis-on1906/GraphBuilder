using GraphBuilder.Models;
using System.Xml.Serialization;
using System.IO;
using System.Xml;


namespace GraphBuilder.Services;
public static class GraphXmlService
{
    public static void Save(out string errorMessage, Graph graph, string filename = "Graph.xml")
    {
        errorMessage = string.Empty;
        try
        {
            var serializer = new XmlSerializer(typeof(Graph));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using var writer = new StreamWriter(filename);
            serializer.Serialize(writer, graph, namespaces);
        }
        catch(Exception ex)
        {
            errorMessage = $"Ошибка при сохранении графа в файл: {ex.Message}";
        }
    }


    public static Graph Load(out string errorMessage, string filename = "Graph.xml")
    {
        errorMessage = string.Empty;
        if (!File.Exists(filename))
        {
            errorMessage = $"Файл не найден: {filename}";
            return null;
        }

        try
        {
            var serializer = new XmlSerializer(typeof(Graph));
            using var reader = new StreamReader(filename);
            var graph = (Graph)serializer.Deserialize(reader);
            if (!ValidationService.ValidateGraph(graph))
            {
                errorMessage = "Ошибка при загрузке графа: некорректный файл";
                return null;
            }
            return graph;
        }
        catch(Exception ex)
        {
            errorMessage = $"Ошибка при загрузке графа: {ex.Message}";
        }
        return null;
    }
}