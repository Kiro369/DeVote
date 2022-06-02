import cv2


def read_img(img_path):
    img = cv2.imread(img_path, 1)
    return img


def display_frame(frame):
    cv2.imshow('Video', frame)
    cv2.waitKey(1)


def display_rec(frame, locations, scale=1):
    '''
    Giving a frame and faces locations on that frame, for drawing a rectangle around these faces

    :param frame: video stream containing faces
    :param locations: locations of faces
    :return: display the result
    '''
    for top, right, bottom, left in locations:
        cv2.rectangle(frame, (left, top), (right, bottom), (0, 0, 255), 2)

    #display_frame(frame)


def calc_geometric_center(point1, point2):
    return int((point1.x + point2.x) / 2), int((point1.y + point2.y) / 2)
