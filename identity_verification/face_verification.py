import face_recognition
import cv2
import numpy as np
from .cfg import *
import time

def get_face_encodings(frame):
    '''
    Given frame and extract faces to return face encoding for each face in the frame.

    :param frame:frame from video streaming
    :return:a list of face encoding, and its locations
    '''
    face_locations = face_recognition.face_locations(frame)
    face_encoding = face_recognition.face_encodings(frame, face_locations)
    return face_encoding, face_locations

def get_id_face_encoding(id_img):
    '''
    getting face encoding from ID front image

    :param id_img: ID front image
    :return: face encoding from ID
    '''
    id_face_encoding = get_face_encodings(id_img)[0]
    return id_face_encoding
    pass

def compare_faces(cammera_connection, ID_face):
    compared_distances = np.empty(num_of_frames)
    iterations = num_of_frames

    process_this_frame = True
    while True:
        ret, frame = cammera_connection.read()

        if process_this_frame:
            temp = get_face_encodings(frame)
            face_encodings = temp[0]
            if len(face_encodings) == 1:
                face_distances = face_recognition.face_distance(ID_face, face_encodings[0])
                compared_distances[iterations-1] = face_distances[0]
                if iterations == 0:
                    break
                else:
                    iterations = iterations - 1

                time.sleep(min(1, delay_time))
                print("True ") if (face_distances[0] <= tolerance) else print("False: ")
            else:
                print("please look at the camera and be alone")


            display_rec(frame, temp[1])

        process_this_frame = not process_this_frame

    cammera_connection.release()
    cv2.destroyAllWindows()

    return compared_distances


    pass

def display_rec(frame, locations, scale=1):
    '''
    Giving a frame and faces locations on that frame, for drawing a rectangle around these faces

    :param frame: video stream containing faces
    :param locations: locations of faces
    :return: display the result
    '''
    for top, right, bottom, left in locations:
        cv2.rectangle(frame, (left, top), (right, bottom), (0, 0, 255), 2)
        # Display the resulting image
    cv2.imshow('Video', frame)
    cv2.waitKey(1)
