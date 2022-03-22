from .face_verification import compare_faces, get_id_face_encoding, compare_faces_frame
from .cfg import tolerance, video_stream_camera
import numpy as np
import cv2



video_capture = cv2.VideoCapture(video_stream_camera-1)

def verify(front_ID, frame=np.array(0)):
    id_face = get_id_face_encoding(front_ID)
    if len(id_face) != 1:
        print("Enter a valid ID front image");return
    if frame.any():
        distance = compare_faces_frame(id_face, frame)
    else:
        distance = compare_faces(video_capture, id_face).mean()
    return True if distance <= tolerance else False

    pass

def verify_id_frame(ID_path, frame_path):
    ID = read_img(ID_path)
    frame = read_img(frame_path)
    return verify(ID, frame)
    pass

def verify_id_frames(ID_path, frames_paths):
    results = np.empty(0, dtype="bool")
    for frame_path in frames_paths:
        results = np.append(results, verify_id_frame(ID_path, frame_path))
    return True if 1-results.mean() <= tolerance else False
    pass

def read_img(img_path):
    img = cv2.imread(img_path, 1);
    return img



