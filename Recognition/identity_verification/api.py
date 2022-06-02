from .face_verification import compare_faces, get_id_face_encoding, compare_faces_frame, compare_faces_without_antispoofing, tts
from .cfg import tolerance, video_stream_camera, num_of_frames
from .helper import read_img
import numpy as np


camera_source = video_stream_camera-1


def verify(front_ID_path, frame=np.array(0)):
    front_ID = read_img(front_ID_path)
    id_face = get_id_face_encoding(front_ID)
    if len(id_face) != 1:
        tts("Invalid_card")
        return False
    if frame.any():
        verified = compare_faces_frame(id_face, frame)[2]
    else:
        verified = compare_faces_without_antispoofing(camera_source, id_face)[0]
    return verified


def verify_id_frame(ID_path, frame_path):
    frame = read_img(frame_path)
    return verify(ID_path, frame)
    pass


def verify_id_frames(ID_path, frames_paths):
    results = np.empty(0, dtype="bool")
    for frame_path in frames_paths:
        results = np.append(results, verify_id_frame(ID_path, frame_path))
    return True if 1-results.mean() <= tolerance else False
    pass


def verify_personality(cam_ref, ID_path, frames_num=num_of_frames):
    front_ID = read_img(ID_path)
    id_face = get_id_face_encoding(front_ID)
    if len(id_face) != 1:
        tts("Invalid_card")
        return False

    result = compare_faces(cam_ref, id_face, frames_num)
    return result
    pass
