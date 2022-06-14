from .cfg import *
from tensorflow.keras.models import model_from_json
import face_recognition
import dlib
import cv2
import numpy as np
import time
import mediapipe
import simpleaudio
import random
import math
import os
from .helper import display_frame, display_rec, calc_geometric_center


dir_path = os.path.dirname(os.path.dirname(__file__))

face_detection = cv2.CascadeClassifier(dir_path+"/models/frontfaces.xml")
movements = ("Right", "Left", "Up", "Down", "Close_eye")

with open(dir_path+"/models/spoofing_model.json", "r") as f:
    loaded_model = f.read()
    model = model_from_json(loaded_model)

model.load_weights(dir_path+'/models/spoofing_model.h5')
detector = dlib.get_frontal_face_detector()
predictor = dlib.shape_predictor(dir_path+'/models/face_landmarks_detector.dat')
media_mesh = mediapipe.solutions.face_mesh
face_mesh = media_mesh.FaceMesh()


def compare_faces(cam_source, ID_face, frames_num=num_of_frames):
    recognition_data = {"embedded": [], "paths": []}
    cam_ref = cv2.VideoCapture(cam_source)
    moves = list(movements)
    tts("Follow_1")
    pass_frames(cam_ref, 10)
    for i in range(num_of_movements):
        random_movement = random.choice(moves)
        correct_move = spoofing_movements(cam_ref, random_movement, recognition_data)
        if not correct_move:
            tts("Follow_2")
            pass_frames(cam_ref, 10)
            correct_move = spoofing_movements(cam_ref, random_movement, recognition_data)
            if not correct_move:
                tts("Sorry")
                pass_frames(cam_ref, 10)
                cv2.destroyAllWindows()
                cam_ref.release()
                return False
        moves.remove(random_movement)
        #print(correct_move)
    pass_frames(cam_ref)
    tts("Smile")
    #time.sleep(1)
    pass_frames(cam_ref, 200)
    dists = np.empty(0)
    for encoding in recognition_data["embedded"]:
        dists = np.append(dists, face_recognition.face_distance(ID_face, encoding[0]))

    saved_encoding = len(recognition_data["embedded"])
    while saved_encoding < frames_num:
        if cam_ref.isOpened():
            ret, frame = cam_ref.read()
            frame = cv2.resize(frame, (600, 450))
            if ret:
                dist, embedded, _ = compare_faces_frame(ID_face, frame)
                if dist == 1:
                    pass_frames(cam_ref)
                else:
                    dists = np.append(dists, dist)
                    save_frame(frame, recognition_data, embedded)
                    saved_encoding += 1
                #display_frame(frame)

    cam_ref.release()
    cv2.destroyAllWindows()
    tts("Processing")
    pass_frames(cam_ref, 10)
    result = (True, recognition_data["paths"]) if dists.mean() <= tolerance else (False,)
    if not result[0]:
        tts("Sorry")
    return result


def spoofing_movements(cam_ref, movement, recognition_data, elapsed_time=10):
    start = time.time()
    dipth_spoof = np.empty(0, dtype=bool)
    moves = np.empty(0)
    if movement == "Close_eye":
        return closing_eye(cam_ref, recognition_data)
    tts(movement)
    pass_frames(cam_ref, 10)
    while cam_ref.isOpened():
        ret, frame = cam_ref.read()
        frame = cv2.resize(frame, (600, 450))
        if ret:
            #if multi_face_spoof(cam_ref, frame):
            #    return False
            #dipth_spoof = np.append(dipth_spoof, is_not_spoofing(frame.copy()))
            frame = cv2.cvtColor(cv2.flip(frame.copy(), 1), cv2.COLOR_BGR2RGB)
            results = face_mesh.process(frame)
            frame = cv2.cvtColor(frame, cv2.COLOR_RGB2BGR)

            img_height, img_width, img_channel = frame.shape
            focal_length = 1 * img_width
            face_3d = []
            face_2d = []
            if results.multi_face_landmarks:
                for face_landmarks in results.multi_face_landmarks:
                    for index, landmark in enumerate(face_landmarks.landmark):
                        if index in (33, 263, 1, 61, 291, 199):
                            x, y = int(landmark.x * img_width), int(landmark.y * img_height)
                            face_2d.append([x, y])
                            face_3d.append([x, y, landmark.z])

                    face_2d = np.array(face_2d, dtype=np.float64)
                    face_3d = np.array(face_3d, dtype=np.float64)

                    cam_matrix = np.array([[focal_length, 0, img_height / 2],
                                           [0, focal_length, img_width / 2],
                                           [0, 0, 1]])

                    distortion_matrix = np.zeros((4, 1), dtype=np.float64)
                    rotational_vectors = cv2.solvePnP(face_3d, face_2d, cam_matrix, distortion_matrix)[1]
                    rotational_matrix = cv2.Rodrigues(rotational_vectors)[0]
                    angles = cv2.RQDecomp3x3(rotational_matrix)[0]
                    x, y, z = map(lambda ele: ele * 360, angles)
                    move = None
                    if y < -10:
                        move = "Left"
                    elif y > 10:
                        move = "Right"
                    elif x < -5:
                        move = "Down"
                    elif x > 10:
                        move = "Up"

                    if move == movement:
                        moves = np.append(moves, move)
                    end = time.time()
                    if (end - start) > elapsed_time or moves.size > 30:
                        # dipth_sp = dipth_spoof.mean() >= .4
                        if verify_movement(moves.size, 15): #dipth
                            pass_frames(cam_ref, 15)
                            #add_embedded(cam_ref, recognition_data)
                            return True
                        return False
        #display_frame(frame)
    return False


def verify_movement(move_len, min_limit, dipth=True):
    return True if move_len > min_limit and dipth else False


def closing_eye(cam_ref, recognition_data, elapsed_time=5):
    start = time.time()
    closed = np.empty(0, dtype=bool)
    dipth_spoof = np.empty(0, dtype=bool)
    tts("Close_eye")
    pass_frames(cam_ref, 10)
    while cam_ref.isOpened():
        ret, frame = cam_ref.read()
        frame = cv2.resize(frame, (600, 450))
        if ret:
            #if multi_face_spoof(cam_ref, frame):
            #    return False
            #dipth_spoof = np.append(dipth_spoof, is_not_spoofing(frame.copy()))
            gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
            faces = detector(gray)
            for face in faces:
                landmarks = predictor(gray, face)
                blinked = verify_blinking(landmarks)
                if blinked:
                    closed = np.append(closed, True)

                end = time.time()
                if (end - start) > elapsed_time or closed.size > 30:
                    # dipth_sp = dipth_spoof.mean() >= .4
                    if verify_movement(closed.size, 15):
                        tts("Open_eye")
                        pass_frames(cam_ref, 10)
                        pass_frames(cam_ref, 15)
                        #add_embedded(cam_ref, recognition_data)
                        return True
                    tts("Must_close")
                    pass_frames(cam_ref, 10)
                    return False

            #display_frame(frame)
    return False


def verify_blinking(landmarks):
    left_point = (landmarks.part(36).x, landmarks.part(36).y)
    right_point = (landmarks.part(39).x, landmarks.part(39).y)
    center_top = calc_geometric_center(landmarks.part(37), landmarks.part(38))
    center_bottom = calc_geometric_center(landmarks.part(41), landmarks.part(40))

    horizontal_len = math.hypot(left_point[0] - right_point[0], left_point[1] - right_point[1])
    vertical_len = math.hypot(center_top[0] - center_bottom[0], center_top[1] - center_bottom[1])
    return horizontal_len / vertical_len > 4


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


def compare_faces_without_antispoofing(cam_source, ID_face, frames_num=num_of_frames):
    compared_distances = np.empty(0)
    cam_ref = cv2.VideoCapture(cam_source)
    while cam_ref.isOpened():
        ret, frame = cam_ref.read()
        locations = [0, iter([(0, 0, 0, 0), ])]
        if ret:
            encodings = get_face_encodings(frame)
            face_encodings = encodings[0]
            locations = encodings[1]
            if len(face_encodings) == 1:
                face_distances = face_recognition.face_distance(ID_face, face_encodings[0])
                compared_distances = np.append(compared_distances, face_distances[0])
                if compared_distances.size > frames_num:
                    break
                pass_frames(cam_ref)

                #time.sleep(min(1, delay_time))
                #print("True ") if (face_distances[0] <= tolerance) else print("False: ")
            else:
                tts("Stand_alone")
                pass_frames(cam_ref, 10)

        #display_rec(frame, locations)

    cam_ref.release()
    cv2.destroyAllWindows()

    return compared_distances.mean() <= tolerance, []


def compare_faces_frame(ID_face, frame):
    face_encodings = get_face_encodings(frame)[0]
    face_distances = face_recognition.face_distance(ID_face, face_encodings[0]) if len(face_encodings) == 1 else [1, ]
    return face_distances[0], face_encodings, face_distances[0] <= tolerance
    pass


def is_there_more_one(frame):
    face_encodings = get_face_encodings(frame)[0]
    return True if len(face_encodings) != 1 else False, face_encodings
    pass


def is_not_spoofing_vid(vid_path):
    '''

    :param vid_path:
    :return:
    '''
    video = cv2.VideoCapture(vid_path)
    if not video.isOpened():
        return False, 1

    predictions = np.empty(0)

    while video.isOpened():
        try:
            ret, frame = video.read()
            if ret:

                gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
                faces = face_detection.detectMultiScale(gray, 1.3, 5)
                for (x, y, w, h) in faces:
                    face = frame[y - 5:y + h + 5, x - 5:x + w + 5]
                    resized_face = cv2.resize(face, (160, 160))
                    resized_face = resized_face.astype("float") / 255.0
                    resized_face = np.expand_dims(resized_face, axis=0)
                    prediction = model.predict(resized_face)[0]
                    predictions = np.append(predictions, prediction)

            else:
                break
        except cv2.error as error:
            print("[Error]: {}".format(error))

    video.release()
    cv2.destroyAllWindows()
    print(predictions)
    return ("True", 0) if predictions.mean() <= .4 else ("False", 0)


def is_not_spoofing(frame):
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    faces = face_detection.detectMultiScale(gray, 1.3, 5)
    prediction = False
    for (x, y, w, h) in faces:
        face = frame[y - 5:y + h + 5, x - 5:x + w + 5]
        resized_face = cv2.resize(face, (160, 160))
        resized_face = resized_face.astype("float") / 255.0
        resized_face = np.expand_dims(resized_face, axis=0)
        prediction   = model.predict(resized_face)[0] <= .4
    return prediction


def pass_frames(cam_ref, number_to_pass=20):
    passed = 0
    while cam_ref.isOpened() and passed < number_to_pass:
        ret, frame = cam_ref.read()
        if ret:
            passed += +1
            #display_frame(frame)
    return cam_ref.read()[1]


def add_embedded(cam_ref, recognition_data):
    start = time.time()
    while cam_ref.isOpened():
        ret, frame = cam_ref.read()
        if ret:
            encodings = is_there_more_one(frame)
            if not encodings[0]:
                save_frame(frame, recognition_data, encodings[1])
                break
            else:
                tts("Stand_alone")
                pass_frames(cam_ref, 10)
        end = time.time()
        if end-start > 2:
            return

    pass


def save_frame(frame, recognition_data, embedded):
    recognition_data["embedded"].append(embedded)
    frame_path = dir_path + "/frame{}.jpg".format(str(len(recognition_data["embedded"])))
    cv2.imwrite(frame_path, frame)
    recognition_data["paths"].append(frame_path)
    return


def multi_face_spoof(cam_ref, frame, tries=1):
    if is_there_more_one(frame)[0]:
        print("please be alone in front of the camera:")
        pass_frames(cam_ref, 25)
        if tries == 3:
            return True
        return multi_face_spoof(cam_ref, cam_ref.read()[1], tries + 1)
    return False


sounds_path = {"Right": "Right.wav", "Left": "Left.wav", "Up": "Up.wav", "Down": "Down.wav",
               "Close_eye": "Close_eye.wav", "Open_eye": "Open_eye.wav", "Follow_1": "Follow_1.wav",
               "Follow_2": "Follow_2.wav", "Invalid_card": "Invalid_ID.wav", "Must_close": "Must_close.wav",
               "Sorry": "Sorry.wav", "Processing": "Processing.wav", "Smile": "Smile.wav",
               "Stand_alone": "Stand_alone.wav", "Look_at_cam": "Look_at_cam.wav"
               }
sounds = {}
for f in sounds_path.keys():
    sounds[f] = simpleaudio.WaveObject.from_wave_file(dir_path+"/data/voice_cmd/face_rec_cmd/"+sounds_path[f])


def tts(command):
    try:
        voice_command = sounds[command].play()
        voice_command.wait_done()
    except KeyError:
        print("key error")
        return
