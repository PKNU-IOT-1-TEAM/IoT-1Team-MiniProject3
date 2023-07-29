import cv2
import numpy as np
import matplotlib.pyplot as plt
import pytesseract

plt.style.use("dark_background")

img_car = cv2.imread("./DSC_9422.jpg")

height, width, channel = img_car.shape

plt.figure(figsize=(12, 10))
plt.imshow(img_car, cmap="gray")
print(height, width, channel)

img_blur = cv2.GaussianBlur(gray, ksize=(5, 5), sigmaX=0)

img_blur_thresh = cv2.adaptiveThreshold(
    img_blur,
    maxValue=255.0,
    adaptiveMethod=cv2.ADATIVE_THESH_GAUSSIAN_C,
    thresholdType=cv2.THRESH_BINARY_INV,
    blockSize=19,
    C=9,
)

# 가우시안 까지 함
