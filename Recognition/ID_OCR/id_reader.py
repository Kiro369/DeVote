import numpy as np
from pytesseract import image_to_string
import cv2
import imutils
import re
import easyocr
from difflib import SequenceMatcher


def id_read(img, gray=False, *, data, face):
    '''

    :param img:
    :param gray:
    :param data:
    :param face:
    :return:
    '''
    image = img if gray else cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    image = cv2.resize(image, (1410, 900))
    thresh = 91
    bi_img = cv2.threshold(image, thresh, 255, cv2.THRESH_BINARY)[1]

    #bi_img = cv2.adaptiveThreshold(image, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 201, 70)
    data_coords = {"front": {"first_name": [(900, 220), (1380, 300)],
                      "full_name" : [(480, 310), (1380, 400)],
                      "address"   : [(480, 420), (1380, 570)],
                      "ID"        : [(620, 685), (1380, 780)]},
            "back": {"expire_date": [(450, 350), (750, 500)]}}

    '''
    for coord in data_coords[face].values():
        bi_img = cv2.rectangle(bi_img, coord[0], coord[1], (0, 0, 0), 2)
    
    cv2.imshow("coord", bi_img)
    '''
    for key, val in data_coords[face].items():
        (x1, y1), (x2, y2) = val
        if key == "ID" or key == "expire_date":
            data[face][key] = "".join(image_to_string(bi_img[y1:y2, x1:x2], lang="hin").split());continue

        data[face][key] = re.sub('[^A-Za-zا-ي0 -9]+', '', image_to_string(bi_img[y1:y2, x1:x2], lang="ara").replace("\n", " "))

    pass


def get_contours(img_edges):
    contours = cv2.findContours(img_edges, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    contours = imutils.grab_contours(contours)
    contours = sorted(contours, key=cv2.contourArea, reverse=True)

    for c in contours:
        approx = cv2.approxPolyDP(c, .01 * cv2.arcLength(c, True), True)
        if len(approx) == 4:
            return approx
    return []

    pass


def id_crop(id_img, gray=False):
    '''

    :param id_img:
    :return:
    '''
    image = id_img if gray else cv2.cvtColor(id_img, cv2.COLOR_BGR2GRAY)
    blurred = cv2.bilateralFilter(image, 33, 40, 40)
    canny = cv2.Canny(blurred, 70, 200)
    #cv2.imshow("canny", canny)
    contours = get_contours(canny)
    if list(contours):
        x, y, w, h = cv2.boundingRect(contours)
        id = id_img.copy()[y:y + h, x: x + w]
        return id
    else:
        print("can't find the ID")
        return
    pass


def id_crop_simple(id_img, gray=1):
    image = id_img if gray != 1 else cv2.cvtColor(id_img, cv2.COLOR_BGR2GRAY)
    image = cv2.threshold(image, 160, 255, cv2.THRESH_BINARY)[1]
    blurred = cv2.blur(image, (3, 3))
    canny = cv2.Canny(blurred, 160, 200)
    # cv2.imshow("canny", canny)

    points = np.argwhere(canny > 0)
    y1, x1 = points.min(axis=0)
    y2, x2 = points.max(axis=0)
    # cv2.imshow("croped", id_img[y1:y2, x1:x2])
    # crop ID and return it
    return id_img[y1:y2, x1:x2]
    pass

def ocr_id(id_path, face):
    id = img_load(id_path)
    info = {"front": {"first_name": "", "full_name": "", "address": "", "ID": ""},
            "back": {"expire_date": ""}}

    id = cv2.resize(id, (800, 600))
    cropped = id_crop(id)
    id_read(cropped, data=info, face="front")
    return info
    pass

def img_load(img_path):
    img = cv2.imread(img_path, 1)
    return cv2.resize(img, (800, 600))

def test_card(card):
    image = cv2.resize(card, (1410, 900))

    data_coords = {"front_test": [(670, 120), (900, 190)],
                   "back_test" : [(750, 350), (990, 500)]}

    data = {"front_test": "",
            "back_test" : ""}


    reader = easyocr.Reader(["ar"])
    for key, val in data_coords.items():
        (x1, y1), (x2, y2) = val
        text_mat = reader.readtext(image[y1:y2, x1:x2])
        data[key] = text_mat[0][-2] if len(text_mat) > 0 else None


    return data
    pass

def is_front_back(img_path):
    img = img_load(img_path)
    cropped = id_crop(img)
    data = [*test_card(cropped).values()]
    if data[0] is None and data[1] is None:
        return "None"
    elif SequenceMatcher(None, data[0], "الشخصية").ratio() >= .60:
        return "Front"
    elif False:
        """call method for checking barcode"""
        return "Back"
    else:
        return "None"


def is_there_card(img_path):
    img = img_load(img_path)
    id = id_crop(img)
    if(id is not None):
      test_cropped = np.array(id.shape[:2]) >= 50
      return test_cropped.all()
    else:
        return False
    pass