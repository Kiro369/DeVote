import numpy as np
from pytesseract import image_to_string
import cv2
import imutils
import re
import easyocr
from difflib import SequenceMatcher
import os
import time
import requests
import json
import simpleaudio
from collections import Counter

dir_path = os.path.dirname(os.path.dirname(__file__))
reader = easyocr.Reader(["ar"])


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
    thresh = 89
    bi_img = cv2.threshold(image, thresh, 255, cv2.THRESH_BINARY)[1]
    #bi_img = image
    #bi_img = cv2.adaptiveThreshold(image, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 81, 21)
    data_coords = {"Front": {"first_name" : [(900, 220), (1380, 300)],
                             "full_name"  : [(480, 310), (1380, 400)],
                             "address"    : [(480, 420), (1380, 570)],
                             "ID"         : [(620, 685), (1380, 780)]},
                   "Back" : {"expire_date": [(450, 350), (750, 500)],
                             "job"        : [(450, 100), (1130, 260)]}}

    '''
    for coord in data_coords[face].values():
        bi_img = cv2.rectangle(bi_img, coord[0], coord[1], (0, 0, 0), 2)

    cv2.imshow("coord", bi_img)
    '''
    for key, val in data_coords[face].items():
        (x1, y1), (x2, y2) = val
        if key == "ID" or key == "expire_date":
            data[face][key] = "".join(image_to_string(bi_img[y1:y2, x1:x2], lang="hin").split())
            continue

        #text_mat = reader.readtext(bi_img[y1:y2, x1:x2])
        #data[face][key] = text_mat[0][-2] if len(text_mat) > 0 else ""
        data[face][key] = re.sub('[^A-Za-zا-ي0 -9]+', '', image_to_string(bi_img[y1:y2, x1:x2], lang="ara").replace("\n", " "))
    if face == "Back":
        ex_date = data[face]["expire_date"]
        if len(ex_date) > 3:
            data[face]["expire_date"] = ex_date[0:4]


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
    temp = id_img.copy()
    image = id_img if gray else cv2.cvtColor(temp, cv2.COLOR_BGR2GRAY)
    #blurred = cv2.GaussianBlur(image, (7, 7), 0)

    blurred = cv2.bilateralFilter(image, 33, 40, 40)
    canny = cv2.Canny(blurred, 120, 200)
    # cv2.imshow("canny", canny)
    contours = get_contours(canny)
    contours = select_contour(contours)
    if list(contours):
        x, y, w, h = cv2.boundingRect(contours)
        id = id_img.copy()[y:y + h, x: x + w]
        return id
    else:
        print("can't find the ID")
        return None
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
    info = {"Front": {"first_name": "", "full_name": "", "address": "", "ID": ""},
            "Back": {"expire_date": "", "job": ""}}

    cropped = id_crop(ID_img)
    #cv2.imshow("crooped", cropped)
    #cv2.waitKey(1)
    if cropped is not None:
        id_read(cropped, data=info, face=face)

    return info
    pass


def img_load(img_ref):
    '''
    helps method for loading image using specific path
    :param img_ref: image path as str or opencv image
    :return: resize loaded image
    '''
    if type(img_ref) is not str:
        img_path = cv2.resize(img_ref, (800, 600))
        return img_path
    img = cv2.imread(img_ref, 1)
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

    #reader = easyocr.Reader(["ar"])
    for key, val in data_coords.items():
        (x1, y1), (x2, y2) = val
        text_mat = reader.readtext(image[y1:y2, x1:x2])
        data[key] = text_mat[0][-2] if len(text_mat) > 0 else None
        #data[key] = image_to_string(image[y1:y2, x1:x2], lang="ara").replace("\n", " ")

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
        return "None"
    elif data[0] is not None and SequenceMatcher(None, data[0], "الشخصية").ratio() >= .60:
        return "Front"
    else:
        barcode = get_barcode(cropped)
        if barcode[0]:
            is_Pdf417 = is_back_API(barcode[1])
            return "Back" if is_Pdf417 else "None"
        else:
            return "None"


def is_there_card(img_path):
    '''
    search if there is a card in a image
    :param img_path: the path of image
    :return: there is a card or Not (True, False)
    '''
    img = img_load(img_path)
    cropped = id_crop(img)

    #if cropped is not None:
     #   cv2.imshow("test", cropped)
      #  cv2.waitKey(1)
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
    '''
    processed_img = preprocess_img(img)
    contours = get_contours(processed_img)[0]
    rect = cv2.minAreaRect(contours)
    box = cv2.cv.BoxPoints(rect) if imutils.is_cv2() else cv2.boxPoints(rect)
    box = np.int0(box)
    x, y, w, h = cv2.boundingRect(box)
    barcode = img.copy()[y:y + h, x: x + w]'''
    path = dir_path + "/barcode.jpg"
    #saved = cv2.imwrite(path, barcode)
    saved = cv2.imwrite(path, img)
    return saved, path


def is_back_API(filePath):
    url = "https://wabr.inliteresearch.com/barcodes"
    formData = {'types': 'Pdf417', 'fields': 'type,length', }
    files = {'file': (filePath, open(filePath, 'rb'), 'application/octet-stream')}
    response = requests.post(url, data=formData, files=files)

    # print(response.text)

    if response.status_code != 200:
        print("Could not sent the Request")
        return False

    resJson = json.loads(response.text)
    Pdf417BarcodeList = resJson["Barcodes"]

    if not len(Pdf417BarcodeList):
        print("No Barcode of type Pdf417 detected")
        return False

    return True


sounds_path = {"Front": "ask_enter_card.wav", "Back": "ask_flip_card.wav", "End": "thanks.wav"}
sounds = {}
for f in sounds_path.keys():
    sounds[f] = simpleaudio.WaveObject.from_wave_file(dir_path+"/data/voice_cmd/card_cmd/"+sounds_path[f])


def scan(frame, face, ter):
    data = ocr_id(frame, face)
    if check_acc(data, face):
        if face == "Front":
            front_path = dir_path + "/front{}.jpg".format(ter)
            cv2.imwrite(front_path, frame)
            data[face]["front_path"] = front_path
        return data[face]
    else:
        return None


def check_acc(data, face):
    if face == "Front":
        return len(data[face]["first_name"]) > 1 and len(data[face]["ID"]) == 14
    else:
        return len(data[face]["expire_date"]) == 4


def pass_frames(cam_ref, number_to_pass=20):
    passed = 0
    while cam_ref.isOpened() and passed < number_to_pass:
        ret, frame = cam_ref.read()
        if ret:
            passed += +1
    return cam_ref.read()[1]


def scan_card(path, execution_time=60):
    face = {"Front": None}  # , "Back": None}
    ter, ids = 0, []
    data = {"Front": []}
    video = cv2.VideoCapture(path)
    if not video.isOpened():
        return False, {}
    elapsed_time = max(execution_time, 60)

    for face_key in face.keys():
        voice_ord = sounds[face_key].play()
        voice_ord.wait_done()
        pass_frames(video, 20)
        start = time.time()
        while video.isOpened():
            ret, frame = video.read()
            if ret:
                is_card = is_there_card(frame)
                if is_card:
                    #front_or_back = is_front_back(frame)
                    if face[face_key] is None:# and front_or_back == face_key:
                        temp = scan(frame, face_key, ter)
                        if temp is not None:
                            data[face_key].append(temp)
                            ids.append(temp["ID"])
                            ter = ter + 1
                            if ter == 5:
                                face[face_key] = True
                                data[face_key] = get_id_data(data[face_key], ids)
                                break

            pass_frames(video, 5)
            end = time.time()
            if (end-start) > elapsed_time:
                terminate(video)
                return False, {}
    terminate(video)
    return all(list(face.values())), data


def terminate(cam_ref):
    cam_ref.release()
    cv2.destroyAllWindows()
    voice_th = sounds["End"].play()
    voice_th.wait_done()


def get_id_data(data, ids):
    counts = Counter(ids)
    max_id = max(counts.items(), key = lambda k : k[1])[0]
    for val in data:
        if val["ID"] == max_id:
            return val