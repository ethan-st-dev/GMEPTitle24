using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GMEPTitle24
{
    public class Scope : INotifyPropertyChanged
    {
        public string id = Guid.NewGuid().ToString();
        public string projectId = string.Empty;
        public int projectScopeId = 1;
        public int gradeStories = 0;
        public bool completeBuildingMethod = false;
        public int conditionedMethodId = 5;
        public int unconditionedMethodId = 5;
        public ObservableCollection<CheckboxItem> OccupancyTypes { get; set; } = new ObservableCollection<CheckboxItem>
        {
            new CheckboxItem { Name = "Auditorium Building", Number = 1, IsSelected = false },
            new CheckboxItem { Name = "Classroom Building", Number = 2, IsSelected = false },
            new CheckboxItem { Name = "Commercial Industrial Building", Number = 3, IsSelected = false },
            new CheckboxItem { Name = "Convention Center Building", Number = 4, IsSelected = false },
            new CheckboxItem { Name = "Data Center Building", Number = 5, IsSelected = false },
            new CheckboxItem { Name = "Financial Institution Building", Number = 6, IsSelected = false },
            new CheckboxItem { Name = "Grocery Store Building", Number = 7, IsSelected = false },
            new CheckboxItem { Name = "Gymnasium Building", Number = 8, IsSelected = false },
            new CheckboxItem { Name = "High Rise Residential", Number = 9, IsSelected = false },
            new CheckboxItem { Name = "Healthcare", Number = 10, IsSelected = false },
            new CheckboxItem { Name = "Hotel/Motel", Number = 11, IsSelected = false },
            new CheckboxItem { Name = "Library Building", Number = 12, IsSelected = false },
            new CheckboxItem { Name = "Medical Clinic Building", Number = 13, IsSelected = false },
            new CheckboxItem { Name = "Multi-family/MF Mixed-user ≥4 stories (includes dormitory, senior living)", Number = 14, IsSelected = false },
            new CheckboxItem { Name = "Office Building", Number = 15, IsSelected = false },
            new CheckboxItem { Name = "Parking Garage", Number = 16, IsSelected = false },
            new CheckboxItem { Name = "Relocatable Public School", Number = 17, IsSelected = false },
            new CheckboxItem { Name = "Religious Facility Building", Number = 18, IsSelected = false },
            new CheckboxItem { Name = "Restaurant/Commercial Kitchen", Number = 19, IsSelected = false },
            new CheckboxItem { Name = "Retail Building", Number = 20, IsSelected = false },
            new CheckboxItem { Name = "School Building", Number = 21, IsSelected = false },
            new CheckboxItem { Name = "Sports Arena Building", Number = 22, IsSelected = false },
            new CheckboxItem { Name = "Support Area", Number = 23, IsSelected = false },
            new CheckboxItem { Name = "Theater Building", Number = 24, IsSelected = false },
            new CheckboxItem { Name = "Warehouse", Number = 25, IsSelected = false },
            new CheckboxItem { Name = "Other (Write In)", Number = 26, IsSelected = false }
        };
        

        public Scope(
            string id,
            string projectId,
            int projectScopeId,
            int gradeStories,
            string occupancyTypeIds
        )
        {
            this.id = id;
            this.projectId = projectId;
            this.projectScopeId = projectScopeId;
            this.gradeStories = gradeStories;

            //Iterating through occupancytypeids to establish checkboxes
            List<int> selectedIds = new List<int>();
            if (!string.IsNullOrEmpty(occupancyTypeIds))
            {
                try
                {
                    selectedIds = JsonSerializer.Deserialize<List<int>>(occupancyTypeIds);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing occupancyTypeIds: {ex.Message}");
                }
            }
            foreach (var typeId in selectedIds)
            {
                var matchingItem = OccupancyTypes.FirstOrDefault(item => item.Number == typeId);
                if (matchingItem != null)
                {
                    matchingItem.IsSelected = true;
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class CheckboxItem
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public bool IsSelected { get; set; }
    }
}
