import numpy as np
from pytesseract import image_to_string
import cv2
import imutils
import re
import easyocr
from difflib import SequenceMatcher
import os

dir_path = os.path.dirname(os.path.dirname(__file__))


def id_read(img, gray=False, *, data, face):
    '''
    reading ID card info
    :param img: image contain ID card
    :param gray: indicator if image passed with grayscale or Not
    :param data: dictionary contain data you want to read as key,value pair
    :param face: indicator for card side (front side , back side)
    :return: dictionary have read data from the card
    '''
    image = img if gray else cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    image = cv2.resize(image, (1410, 900))
    thresh = 91
    bi_img = cv2.threshold(image, thresh, 255, cv2.THRESH_BINARY)[1]

    # bi_img = cv2.adaptiveThreshold(image, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 201, 70)
    data_coords = {"front": {"first_name": [(900, 220), (1380, 300)],
                             "full_name": [(480, 310), (1380, 400)],
                             "address": [(480, 420), (1380, 570)],
                             "ID": [(620, 685), (1380, 780)]},
                   "back": {"expire_date": [(450, 350), (750, 500)]}}

    '''
    for coord in data_coords[face].values():
        bi_img = cv2.rectangle(bi_img, coord[0], coord[1], (0, 0, 0), 2)

    cv2.imshow("coord", bi_img)
    '''
    for key, val in data_coords[face].items():
        (x1, y1), (x2, y2) = val
        if key == "ID" or key == "expire_date":
            data[face][key] = "".join(image_to_string(bi_img[y1:y2, x1:x2], lang="hin").split());
            continue

        data[face][key] = re.sub('[^A-Za-zا-ي0 -9]+', '',
                                 image_to_string(bi_img[y1:y2, x1:x2], lang="ara").replace("\n", " "))

    pass


def get_contours(img_edges):
    '''
    find contours of different region of image helps us to crop it
    :param img_edges: preprocessed image ready for extract contours from it
    :return: sorted list of contours py Area
    '''
    contours = cv2.findContours(img_edges, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    contours = imutils.grab_contours(contours)
    contours = sorted(contours, key=cv2.contourArea, reverse=True)
    return contours

    pass


def select_contour(contours):
    '''
    selecting contours achieve ID card criteria
    :param contours: list of contours
    :return: a list contain selected contours
    '''
    for c in contours:
        approx = cv2.approxPolyDP(c, .01 * cv2.arcLength(c, True), True)
        if len(approx) == 4:
            return approx
    return []


def id_crop(id_img, gray=False):
    '''
    crop the ID card from the Image
    :param id_img: image contain the ID
    :param gray: indicator if image passed with grayscale or Not
    :return: cropped ID
    '''
    image = id_img if gray else cv2.cvtColor(id_img, cv2.COLOR_BGR2GRAY)
    blurred = cv2.bilateralFilter(image, 33, 40, 40)
    canny = cv2.Canny(blurred, 70, 200)
    # cv2.imshow("canny", canny)
    contours = get_contours(canny)
    contours = select_contour(contours)
    if list(contours):
        x, y, w, h = cv2.boundingRect(contours)
        id = id_img.copy()[y:y + h, x: x + w]
        return id
    else:
        print("can't find the ID")
        return
    pass


def id_crop_simple(id_img, gray=False):
    '''
    simplest method for cropping ID but less accuracy
    :param id_img: image contain the ID
    :param gray: indicator if image passed with grayscale or Not
    :return: cropped image
    '''
    image = id_img if gray else cv2.cvtColor(id_img, cv2.COLOR_BGR2GRAY)
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
    '''
    preparing card image and reading it
    :param id_path: image contain the ID card
    :param face: indicate the side of the card
    :return: extracting data from the card
    '''
    ID_img = img_load(id_path)
    info = {"front": {"first_name": "", "full_name": "", "address": "", "ID": ""},
            "back": {"expire_date": ""}}

    cropped = id_crop(ID_img)
    id_read(cropped, data=info, face=face)
    return info
    pass


def img_load(img_path):
    '''
    helps method for loading image using specific path
    :param img_path: image path
    :return: resize loaded image
    '''
    img = cv2.imread(img_path, 1)
    return cv2.resize(img, (800, 600))


def test_card(card):
    '''
    read specific data from different side of the card
    :param card: cropped ID card
    :return: data from the front,back side if any
    '''
    image = cv2.resize(card, (1410, 900))

    data_coords = {"front_test": [(670, 120), (900, 190)],
                   "back_test": [(750, 350), (990, 500)]}

    data = {"front_test": "",
            "back_test": ""}

    reader = easyocr.Reader(["ar"])
    for key, val in data_coords.items():
        (x1, y1), (x2, y2) = val
        text_mat = reader.readtext(image[y1:y2, x1:x2])
        data[key] = text_mat[0][-2] if len(text_mat) > 0 else None

    return data
    pass


def is_front_back(img_path):
    '''
    indicating if the ID is front side or back side
    :param img_path: the path of the card image
    :return: tuple 1st element is the card side (Front, Back, None)
                   2nd element is the path for barcode from the back side if any
    '''
    img = img_load(img_path)
    cropped = id_crop(img)
    data = [*test_card(cropped).values()]
    if data[0] is None and data[1] is None:
        return "None", ""
    elif data[0] is not None and SequenceMatcher(None, data[0], "الشخصية").ratio() >= .60:
        return "Front", ""
    else:
        barcode = get_barcode(cropped)
        if barcode[0]:
            return "Back", barcode[1]
        else:
            return "None", ""


def is_there_card(img_path):
    '''
    search if there is a card in a image
    :param img_path: the path of image
    :return: there is a card or Not (True, False)
    '''
    img = img_load(img_path)
    cropped = id_crop(img)
    if cropped is not None:
        test_cropped = np.array(cropped.shape[:2]) >= 50
        return test_cropped.all()
    else:
        return False
    pass


def preprocess_img(img):
    '''
    preparing image and located different regions in the image to helps to crop them easy
    :param img: image have regions we want
    :return: different image regions colored with white
    '''
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    ddepth = cv2.cv.CV_32F if imutils.is_cv2() else cv2.CV_32F
    gradX = cv2.Sobel(gray, ddepth=ddepth, dx=1, dy=0, ksize=-1)
    gradY = cv2.Sobel(gray, ddepth=ddepth, dx=0, dy=1, ksize=-1)
    gradient = cv2.convertScaleAbs(cv2.subtract(gradX, gradY))
    blurred = cv2.blur(gradient, (6, 6))
    bi_image = cv2.threshold(blurred, 120, 255, cv2.THRESH_BINARY)[1]
    kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (21, 7))
    # do some erosions and dilations process for helps us to located regions
    result_image = cv2.morphologyEx(bi_image, cv2.MORPH_CLOSE, kernel)
    result_image = cv2.erode(result_image, None, iterations=7)
    result_image = cv2.dilate(result_image, None, iterations=7)
    return result_image


def get_barcode(img):
    '''
    detecting 'PDF417' barcode, cropped it,
    :param img: image may contain a barcode
    :return: a tuple with 1st element indicate if there is a barcode or Not
    and the path to the cropped barcode
    '''
    processed_img = preprocess_img(img)
    contours = get_contours(processed_img)[0]
    rect = cv2.minAreaRect(contours)
    box = cv2.cv.BoxPoints(rect) if imutils.is_cv2() else cv2.boxPoints(rect)
    box = np.int0(box)
    x, y, w, h = cv2.boundingRect(box)
    barcode = img.copy()[y:y + h, x: x + w]
    path = dir_path + "/barcode.jpg"
    saved = cv2.imwrite(path, barcode)
    return saved, path
