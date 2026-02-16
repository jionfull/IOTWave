from PIL import Image, ImageDraw

"""
故障占用纹理生成脚本
独立运行生成故障占用纹理
"""

def create_fault_occupied_texture():
    """创建故障占用纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 粉蓝色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#FFC0CB")
    
    return img

def main():
    """主函数"""
    print("生成故障占用纹理...")
    img = create_fault_occupied_texture()
    img.save("fault_occupied.png")
    print("故障占用纹理已生成: fault_occupied.png")

if __name__ == "__main__":
    main()