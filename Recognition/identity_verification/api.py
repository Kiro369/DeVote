from .face_verification import compare_faces, get_id_face_encoding, compare_faces_frame, compare_faces_without_antispoofing, tts
from .cfg import tolerance, video_stream_camera, num_of_frames
from .helper import read_img
import numpy as np


camera_source = video_stream_camera-1


def verify(front_ID_path, frame=np.array(0)):
    '''
    verifying card face with a live stream video or a frame face without any spoofing detecting.

    :param front_ID_path: the id card path.
    :param frame: frame if any.
    :return: identification result.
    '''
    front_ID = read_img(front_ID_path)
    id_face = get_id_face_encoding(front_ID) if front_ID is not None else None
    if id_face is None or len(id_face) != 1:
        return False
    if frame.any():
        verified = compare_faces_frame(id_face, frame)[2]
    else:
        verified = compare_faces_without_antispoofing(camera_source, id_face)[0]
    return verified


def verify_id_frame(ID_path, frame_path):
    '''
    identify card face with a frame.

    :param ID_path: id card path.
    :param frame_path: frame path.
    :return: identification result.
    '''
    frame = read_img(frame_path)
    return verify(ID_path, frame) if frame is not None else False
    pass


def verify_id_frames(ID_path, frames_paths):
    '''
    identify card face with some frame.

    :param ID_path: id card path.
    :param frames_paths: list of frames.
    :return: identification result.
    '''
    results = np.empty(0, dtype="bool")
    for frame_path in frames_paths:
        results = np.append(results, verify_id_frame(ID_path, frame_path))
    return True if 1-results.mean() <= tolerance else False
    pass


def verify_personality(cam_source, ID_path, frames_num=num_of_frames):
    '''
    identify id card face with a live stream video with voice command and spoofing detecting.

    :param cam_source: camera source.
    :param ID_path: id card path.
    :param frames_num: number of frames to save.
    :return: identification result.
    '''
    front_ID = read_img(ID_path)
    id_face = get_id_face_encoding(front_ID) if front_ID is not None else None
    if id_face is None or len(id_face) != 1:
        tts("Invalid_card")
        return False, []

    result = compare_faces(cam_source, id_face, frames_num)
    return result
    pass
