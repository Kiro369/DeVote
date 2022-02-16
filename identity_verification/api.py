from .face_verification import compare_faces, get_id_face_encoding
from .cfg import tolerance, video_stream_camera
import cv2



video_capture = cv2.VideoCapture(video_stream_camera-1)

def verify(front_ID):
    id_face = get_id_face_encoding(front_ID)
    if len(id_face) != 1:
        print("Enter a valid ID front image");return

    distance = compare_faces(video_capture, id_face).mean()
    return True if distance <= tolerance else False

    pass