import cv2


def read_img(img_path):
    '''
    load image from the path.

    :param img_path: path to image.
    :return: loaded image.
    '''
    img = cv2.imread(img_path, 1)
    return img


def display_frame(frame):
    '''
    show the frame.

    :param frame: frame want to display.
    :return: displaying frame.
    '''
    cv2.imshow('Video', frame)
    cv2.waitKey(1)


def display_rec(frame, locations, scale=1):
    '''
    Giving a frame and faces locations on that frame, for drawing a rectangle around these faces.

    :param frame: video stream containing faces.
    :param locations: locations of faces.
    :return: display the result.
    '''
    for top, right, bottom, left in locations:
        cv2.rectangle(frame, (left, top), (right, bottom), (0, 0, 255), 2)

    #display_frame(frame)


def calc_geometric_center(point1, point2):
    '''
    calc the center between two point.

    :param point1: first point coords.
    :param point2: second point coords.
    :return: the center point.
    '''

    return int((point1.x + point2.x) / 2), int((point1.y + point2.y) / 2)
