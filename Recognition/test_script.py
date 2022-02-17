import cv2
import ID_OCR as test
import identity_verification as idv


image = cv2.imread("img/front.jpg", 1)
image = cv2.resize(image, (800, 600))
cropped = test.id_crop(image)
# cv2.imshow("cropped", cropped)

verif = idv.verify(cropped)
print(verif)

info = {"front": {"first_name": "", "full_name": "", "address": "", "ID": ""},
                      "back" : {"expire_date": ""}}
data = test.id_read(cropped, data=info, face="front")
print(info)





cv2.waitKey(0)

