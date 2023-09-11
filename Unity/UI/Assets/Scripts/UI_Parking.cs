using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Parking : MonoBehaviour
{
	public Image Parking_1_Image;
    public Image Parking_2_Image;
    public Image Parking_3_Image;
    public Image Parking_4_Image;
    public Image Parking_5_Image;
    public Image Parking_6_Image;
    public Image Parking_7_Image;
    public Image Parking_8_Image;

	private List<Image> images = new List<Image>();

	private string[] parkingReservationStatus;

	// Start is called before the first frame update
	void Start()
    {
		images.Add(Parking_1_Image);
		images.Add(Parking_2_Image);
		images.Add(Parking_3_Image);
		images.Add(Parking_4_Image);
		images.Add(Parking_5_Image);
		images.Add(Parking_6_Image);
		images.Add(Parking_7_Image);
		images.Add(Parking_8_Image);
	}

	// Update is called once per frame
	void Update()
	{
		parkingReservationStatus = Commons.parkingStatusData.reservation_status;

		int loop = 0;
		foreach (var item in Commons.parkingStatusData.reservation_status)
		{
			if (Commons.parkingStatusData.reservation_status[loop] == "0")
			{
				Color newColor = new Color32(194, 194, 194, 255); // Red color
				ApplyColorToImage(images[loop], newColor);
			}
			else if (Commons.parkingStatusData.reservation_status[loop] == "1")
			{
				Color newColor = new Color32(0, 200, 0, 255); // Green color
				ApplyColorToImage(images[loop], newColor);
			}
			else if (Commons.parkingStatusData.reservation_status[loop] == "2")
			{
				Color newColor = new Color32(235, 160, 85, 255); // Orange color
				ApplyColorToImage(images[loop], newColor);
			}
			else if (Commons.parkingStatusData.reservation_status[loop] == "3")
			{
				Color newColor = new Color32(235, 67, 85, 255); // Red color
				ApplyColorToImage(images[loop], newColor);
			}
			loop++;
		}
	}

	// Helper method to apply color to an image
	void ApplyColorToImage(Image image, Color color)
	{
		if (image != null)
		{
			image.color = color;
		}
	}
}
